using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grammophone.Storage.Azure
{
	/// <summary>
	/// <see cref="IStorageProvider"/> implementation for Azure.
	/// It is thread-safe as required by the contract.
	/// </summary>
	public class AzureStorageProvider : IStorageProvider
	{
		#region Construction

		/// <summary>
		/// Create.
		/// </summary>
		/// <param name="connectionString">The Azure connection string.</param>
		/// <param name="urlBase">The URL base of the files provided by the azure account.</param>
		public AzureStorageProvider(string connectionString, string urlBase)
		{
			if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
			if (urlBase == null) throw new ArgumentNullException(nameof(urlBase));

			this.URLBase = urlBase;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// The URL base of the files provided by the azure account.
		/// </summary>
		public string URLBase { get; private set; }

		/// <summary>
		/// The Azure connection string.
		/// </summary>
		public string ConnectionString { get; private set; }

		#endregion

		#region Public methods

		/// <summary>
		/// Get a client for file operations.
		/// </summary>
		public IStorageClient GetClient()
		{
			return new AzureStorageClient(this.ConnectionString, this);
		}

		#endregion
	}
}
