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

		#endregion

		#region Public methods

		public async Task<Stream> OpenReadAsync()
		{
			return await cloudBlockBlob.OpenReadAsync();
		}

		public async Task<Stream> OpenWriteAsync()
		{
			return await cloudBlockBlob.OpenWriteAsync();
		}

		public async Task DownloadToStreamAsync(Stream stream)
		{
			if (stream == null) throw new ArgumentNullException(nameof(stream));

			await cloudBlockBlob.DownloadToStreamAsync(stream);
		}

		#endregion
	}
}
