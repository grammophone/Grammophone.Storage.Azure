using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Grammophone.Storage.Azure
{
	/// <summary>
	/// Azure implementation of a <see cref="IStorageProvider"/>.
	/// </summary>
	public class AzureStorageProvider : IStorageProvider
	{
		#region Private fields

		private CloudBlobClient cloudBlobClient;

		#endregion

		#region Construction

		/// <summary>
		/// Create.
		/// </summary>
		/// <param name="connectionString">The connection string to an Azure storage account.</param>
		public AzureStorageProvider(string connectionString)
		{
			if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

			var account = CloudStorageAccount.Parse(connectionString);

			cloudBlobClient = account.CreateCloudBlobClient();
		}

		#endregion

		#region Public methods

		public async Task<IStorageContainer> GetContainerAsync(string containerName)
		{
			if (containerName == null) throw new ArgumentNullException(nameof(containerName));

			var cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);

			if (!(await cloudBlobContainer.ExistsAsync())) return null;

			return new AzureStorageContainer(cloudBlobContainer);
		}

		#endregion
	}
}
