using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorage.Configuration
{

    public sealed class AzureStorageOptions
    {
        public const string SectionName = "AzureStorage";

        public required string ConnectionString { get; init; }

        public string AvatarsContainer { get; init; } = "avatars";

        public string CommentImagesContainer { get; init; } = "comment-images";

        public string CommentFilesContainer { get; init; } = "comment-files";
    }
}