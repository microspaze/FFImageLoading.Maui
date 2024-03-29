﻿using System;
using Xunit;
using System.Threading.Tasks;
using FFImageLoading;
using FFImageLoading.Mock;

namespace FFImageLoading.Tests.Cache
{
    public class MemoryCacheTests : BaseTests
    {
        [Fact]
        public Task CanAddGet()
        {
            var memoryCache = MockImageCache.Instance;
            var key = Guid.NewGuid().ToString();
            byte[] bytes = new byte[] { 00, 01, 00, 01 };
            var bitmap = new Mock.MockBitmap();
            var imageInfo = new Work.ImageInformation();
            memoryCache.Add(key, imageInfo, bitmap);
            var found = memoryCache.Get(key);
            Assert.NotNull(found);
            Assert.Equal(found.Item1, bitmap);
            Assert.Equal(found.Item2, imageInfo);

            return Task.FromResult(true);
        }
    }
}
