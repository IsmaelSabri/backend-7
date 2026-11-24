using Amazon.S3;
using Images.Collections;
using Images.Models;
using Images.Dto;
using Images.Services.Impl;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;
using Xunit;

public sealed class ImageManagerTests
{
    [Fact]
    public async Task EnergyCertificateResponseExposesItsSingleUrl()
    {
        using var s3 = new R2StorageTests.FakeS3();
        var collection = new FakeCollection();
        var manager = CreateManager(collection, s3);
        var uploaded = await manager.UploadAsync(File(), "owner-1", "EnergyCertImage", default);
        Assert.Equal(ImageStatuses.Processing, uploaded.Status);
        Assert.Null(uploaded.FullUrl);
        collection.Items.Single().Status = ImageStatuses.Ready;
        var fetched = await manager.GetAsync(uploaded.Id, default);
        Assert.NotNull(fetched);
        Assert.NotNull(fetched.FullUrl);
        Assert.EndsWith(".webp", fetched.FullUrl);
        Assert.Null(fetched.ThumbnailUrl);
        Assert.Null(fetched.CardUrl);
        Assert.Single(s3.Uploads);
        Assert.True(await manager.DeleteAsync(uploaded.Id, default));
        Assert.Empty(s3.Uploads);
    }

    [Fact]
    public async Task ProfileResponseContainsBothProfileUrls()
    {
        using var s3 = new R2StorageTests.FakeS3();
        var collection = new FakeCollection();
        var manager = CreateManager(collection, s3);
        var uploaded = await manager.UploadAsync(File(), "owner-1", "ProfileImage", default);
        Assert.Equal(ImageStatuses.Processing, uploaded.Status);
        Assert.Null(uploaded.AvatarUrl);
        collection.Items.Single().Status = ImageStatuses.Ready;
        var fetched = await manager.GetAsync(uploaded.Id, default);
        Assert.NotNull(fetched);
        Assert.EndsWith(".avatar.webp", fetched.AvatarUrl);
        Assert.EndsWith(".webp", fetched.CropperUrl);
        Assert.Null(fetched.FullUrl);
    }

    [Fact]
    public async Task UploadOrdersImagesAndDeleteFailureKeepsRecord()
    {
        using var s3 = new R2StorageTests.FakeS3();
        var collection = new FakeCollection();
        var manager = CreateManager(collection, s3);
        var first = await manager.UploadAsync(File(), "owner-1", "HomeImages", default);
        var second = await manager.UploadAsync(File(), "owner-1", "HomeImages", default);
        Assert.True(first.IsCover);
        Assert.Equal(0, first.Order);
        Assert.False(second.IsCover);
        Assert.Equal(1, second.Order);
        s3.FailOnDelete = true;
        await Assert.ThrowsAsync<AmazonS3Exception>(() => manager.DeleteAsync(first.Id, default));
        Assert.Equal(2, collection.Items.Count);
        s3.FailOnDelete = false;
        Assert.True(await manager.DeleteAsync(first.Id, default));
        Assert.Single(collection.Items);
    }

    [Theory]
    [InlineData("HomeImages")]
    [InlineData("EnergyCertImage")]
    public async Task SaveFailureRetainsOriginalForAmbiguousCommitAndInvalidOwnerIsRejected(string ownerType)
    {
        using var s3 = new R2StorageTests.FakeS3();
        var collection = new FakeCollection { FailOnAdd = true };
        var manager = CreateManager(collection, s3);
        await Assert.ThrowsAsync<InvalidOperationException>(() => manager.UploadAsync(File(), "owner-1", ownerType, default));
        Assert.Single(s3.Uploads);
        Assert.StartsWith("raw/", s3.Uploads.Keys.Single());
        await Assert.ThrowsAsync<ArgumentException>(() => manager.UploadAsync(File(), "../escape", ownerType, default));
    }

    [Fact]
    public async Task SettingCoverUnsetsPreviousCover()
    {
        using var s3 = new R2StorageTests.FakeS3();
        var collection = new FakeCollection();
        var manager = CreateManager(collection, s3);
        var first = await manager.UploadAsync(File(), "owner-1", "HomeImages", default);
        var second = await manager.UploadAsync(File(), "owner-1", "HomeImages", default);
        Assert.True(await manager.UpdateAsync(second.Id, new ImageUpdateRequest { IsCover = true }, default));
        Assert.False(collection.Items.Single(i => i.Id == first.Id).IsCover);
        Assert.True(collection.Items.Single(i => i.Id == second.Id).IsCover);
    }

    [Fact]
    public void OwnerQueryReturnsEveryMatchingImageWithoutPagination()
    {
        var sieve = new SieveProcessor(Options.Create(new SieveOptions()));
        var images = Enumerable.Range(0, 200)
            .Select(i => new Image { Id = Guid.NewGuid(), OwnerId = "owner-1", OwnerType = "HomeImages", StorageKey = $"{i}.webp" })
            .Append(new Image { Id = Guid.NewGuid(), OwnerId = "owner-10", OwnerType = "HomeImages", StorageKey = "other.webp" });
        var result = sieve.Apply(new SieveModel { Filters = "ownerId==owner-1" }, images.AsQueryable()).ToList();
        Assert.Equal(200, result.Count);
    }

    private static ImageManager CreateManager(FakeCollection collection, R2StorageTests.FakeS3 s3) =>
        new(collection, R2StorageTests.CreateStorage(s3), new SieveProcessor(Options.Create(new SieveOptions())),
            NullLogger<ImageManager>.Instance);

    private static IFormFile File()
    {
        var bytes = R2StorageTests.CreateImage(100, 50);
        return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "files", "test.jpg");
    }

    private sealed class FakeCollection : IImageCollection
    {
        public List<Image> Items { get; } = [];
        public bool FailOnAdd { get; init; }
        public Task<Image?> GetByIdAsync(Guid id, CancellationToken ct) => Task.FromResult(Items.Find(i => i.Id == id));
        public Task<Image?> GetByNameAsync(string name, CancellationToken ct) => Task.FromResult(Items.Find(i => i.AltText == name));
        public Task<List<Image>> GetByOwnerAsync(string id, string[] types, CancellationToken ct) =>
            Task.FromResult(Items.Where(i => i.OwnerId == id && types.Contains(i.OwnerType)).ToList());
        public Task AddAsync(Image image, CancellationToken ct)
        {
            if (FailOnAdd) throw new InvalidOperationException("simulated database failure");
            Items.Add(image);
            return Task.CompletedTask;
        }
        public Task UpdateAsync(Image image, CancellationToken ct) => Task.CompletedTask;
        public Task DeleteAsync(Image image, CancellationToken ct) { Items.Remove(image); return Task.CompletedTask; }
        public async Task<bool> WithLockedImageAsync(Guid id, Func<Image, Task> action, CancellationToken ct)
        {
            var image = Items.Find(item => item.Id == id);
            if (image == null) return false;
            await action(image);
            return true;
        }
        public IQueryable<Image> GetPagedImages() => Items.AsQueryable();
    }
}
