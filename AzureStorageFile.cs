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
	/// Azure implementation of a <see cref="IStorageFile"/>.
	/// </summary>
	public class AzureStorageFile : IStorageFile
	{
		#region Private fields

		private CloudBlockBlob cloudBlockBlob;

		#endregion

		#region Construction

		internal AzureStorageFile(CloudBlockBlob cloudBlockBlob, AzureStorageContainer container)
		{
			if (cloudBlockBlob == null) throw new ArgumentNullException(nameof(cloudBlockBlob));
			if (container == null) throw new ArgumentNullException(nameof(container));

			this.cloudBlockBlob = cloudBlockBlob;

			this.Container = container;
		}

		#endregion

		#region Public properties

		public AzureStorageContainer Container { get; private set; }

		IStorageContainer IStorageFile.Container => this.Container;

		public string ContentType => cloudBlockBlob.Properties.ContentType;

		public DateTimeOffset LastModificationDate => cloudBlockBlob.Properties.LastModified.Value;

		public string Name => cloudBlockBlob.Name;

		public Uri URI => cloudBlockBlob.Uri;

		public IDictionary<string, string> Metadata
		{
			get
			{
				cloudBlockBlob.FetchAttributes();

				return cloudBlockBlob.Metadata;
			}
		}

		#endregion

		#region Public methods

		public async Task<Stream> OpenReadAsync()
		{
			return await cloudBlockBlob.OpenReadAsync(
				null, 
				this.Container.Client.Provider.CreateDefaultReadRequestOptions(), 
				null);
		}

		public async Task<Stream> OpenWriteAsync(bool encrypt)
		{
			return await cloudBlockBlob.OpenWriteAsync(
				null,
				this.Container.Client.Provider.CreateDefaultWriteRequestOptions(encrypt),
				null);
		}

		public async Task DownloadToStreamAsync(Stream stream)
		{
			if (stream == null) throw new ArgumentNullException(nameof(stream));

			await cloudBlockBlob.DownloadToStreamAsync(
				stream,
				null,
				this.Container.Client.Provider.CreateDefaultReadRequestOptions(),
				null);
		}

		public async Task UploadFromStreamAsync(Stream stream, bool encrypt)
		{
			if (stream == null) throw new ArgumentNullException(nameof(stream));

			await cloudBlockBlob.UploadFromStreamAsync(
				stream, 
				null, 
				this.Container.Client.Provider.CreateDefaultWriteRequestOptions(encrypt),
				null);

			await cloudBlockBlob.FetchAttributesAsync();
		}

		public async Task SaveMetadataAsync()
		{
			await cloudBlockBlob.SetMetadataAsync();
		}

		#endregion
	}
}
