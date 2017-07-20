using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Grammophone.Storage.Azure
{
	/// <summary>
	/// Azure implementation of a <see cref="IStorageContainer"/>.
	/// </summary>
	public class AzureStorageContainer : IStorageContainer
	{
		#region Private fields

		private CloudBlobContainer cloudBlobContainer;

		#endregion

		#region Construction

		internal AzureStorageContainer(CloudBlobContainer cloudBlobContainer, AzureStorageClient client)
		{
			if (cloudBlobContainer == null) throw new ArgumentNullException(nameof(cloudBlobContainer));
			if (client == null) throw new ArgumentNullException(nameof(client));

			this.cloudBlobContainer = cloudBlobContainer;

			this.Client = client;
		}

		#endregion

		#region Public properties

		public AzureStorageClient Client { get; private set; }

		IStorageClient IStorageContainer.Client => this.Client;

		public string Name => cloudBlobContainer.Name;

		public Uri URI => cloudBlobContainer.Uri;

		#endregion

		#region Public methods

		public async Task<IStorageFile> CreateFileAsync(string filename, string contentType, bool overwrite = true)
		{
			if (filename == null) throw new ArgumentNullException(nameof(filename));
			if (contentType == null) throw new ArgumentNullException(nameof(contentType));

			var blob = cloudBlobContainer.GetBlockBlobReference(filename);

			if (!overwrite)
			{
				if (!(await blob.ExistsAsync()))
				{
					throw new StorageException($"The file '{filename}' already exists.");
				}
			}

			blob.Properties.ContentType = contentType;

			return new AzureStorageFile(blob, this);
		}

		public async Task<bool> DeleteFileAsync(string filename)
		{
			if (filename == null) throw new ArgumentNullException(nameof(filename));

			var blob = cloudBlobContainer.GetBlockBlobReference(filename);

			return await blob.DeleteIfExistsAsync();
		}

		public async Task<bool> FileExistsAsync(string filename)
		{
			if (filename == null) throw new ArgumentNullException(nameof(filename));

			var blob = cloudBlobContainer.GetBlockBlobReference(filename);

			return await blob.ExistsAsync();
		}

		public async Task<IStorageFile> GetFileAsync(string filename)
		{
			if (filename == null) throw new ArgumentNullException(nameof(filename));

			var blob = cloudBlobContainer.GetBlockBlobReference(filename);

			if (!(await blob.ExistsAsync())) return null;

			await blob.FetchAttributesAsync();

			return new AzureStorageFile(blob, this);
		}

		#endregion
	}
}
