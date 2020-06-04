using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace Grammophone.Storage.Azure
{
	/// <summary>
	/// Azure implementation of a <see cref="IStorageClient"/>.
	/// </summary>
	public class AzureStorageClient : IStorageClient
	{
		#region Private fields

		private CloudBlobClient cloudBlobClient;

		#endregion

		#region Construction

		/// <summary>
		/// Create.
		/// </summary>
		/// <param name="connectionString">The connection string to an Azure storage account.</param>
		/// <param name="provider">The provider creating this client.</param>
		internal AzureStorageClient(string connectionString, AzureStorageProvider provider)
		{
			if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
			if (provider == null) throw new ArgumentNullException(nameof(provider));

			var account = CloudStorageAccount.Parse(connectionString);

			cloudBlobClient = account.CreateCloudBlobClient();

			this.Provider = provider;
		}

		#endregion

		#region Public properties

		public AzureStorageProvider Provider { get; private set; }

		IStorageProvider IStorageClient.Provider => this.Provider;

		#endregion

		#region Public methods

		public IStorageContainer GetContainer(string containerName)
		{
			if (containerName == null) throw new ArgumentNullException(nameof(containerName));

			var cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);

			if (!cloudBlobContainer.Exists()) return null;

			return new AzureStorageContainer(cloudBlobContainer, this);
		}

		public async Task<IStorageContainer> GetContainerAsync(string containerName)
		{
			if (containerName == null) throw new ArgumentNullException(nameof(containerName));

			var cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);

			if (!(await cloudBlobContainer.ExistsAsync())) return null;

			return new AzureStorageContainer(cloudBlobContainer, this);
		}

		#endregion
	}
}
