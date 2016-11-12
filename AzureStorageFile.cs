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

		private AzureStorageContainer container;

		#endregion

		#region Construction

		internal AzureStorageFile(CloudBlockBlob cloudBlockBlob, AzureStorageContainer container)
		{
			if (cloudBlockBlob == null) throw new ArgumentNullException(nameof(cloudBlockBlob));
			if (container == null) throw new ArgumentNullException(nameof(container));

			this.cloudBlockBlob = cloudBlockBlob;
			this.container = container;
		}

		#endregion

		#region Public properties

		public IStorageContainer Container
		{
			get
			{
				return container;
			}
		}

		public string ContentType
		{
			get
			{
				return cloudBlockBlob.Properties.ContentType;
			}
		}

		public DateTimeOffset LastModificationDate
		{
			get
			{
				return cloudBlockBlob.Properties.LastModified.Value;
			}
		}

		public string Name
		{
			get
			{
				return cloudBlockBlob.Name;
			}
		}

		public Uri URI
		{
			get
			{
				return cloudBlockBlob.Uri;
			}
		}

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

		#endregion

		#region Private methods

		#endregion
	}
}
