using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.WindowsAzure.Storage.Blob;

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
		/// <param name="base64EncryptionKey">The key to use, in base64, when files are encrypted.</param>
		public AzureStorageProvider(string connectionString, string urlBase, string base64EncryptionKey)
		{
			if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
			if (urlBase == null) throw new ArgumentNullException(nameof(urlBase));
			if (base64EncryptionKey == null) throw new ArgumentNullException(nameof(base64EncryptionKey));

			this.URLBase = urlBase;

			try
			{
				this.EncryptionKey = Convert.FromBase64String(base64EncryptionKey);
			}
			catch (FormatException ex)
			{
				throw new ArgumentException(
					"The encryption key is not a valid base64 string.", 
					nameof(base64EncryptionKey), 
					ex);
			}

			switch (this.EncryptionKey.Length)
			{
				case 128 >> 3:
				case 192 >> 3:
				case 256 >> 3:
				case 384 >> 3:
				case 512 >> 3:
					break;

				default:
					throw new ArgumentException(
						$"The encryption key length is {this.EncryptionKey.Length << 3} bits but it must be 128, 192, 256, 384 or 512.", 
						nameof(base64EncryptionKey));
			}
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

		#region Protected properties

		/// <summary>
		/// The encryption key specified in the constructor,
		/// used when files are encrypted.
		/// </summary>
		protected internal byte[] EncryptionKey { get; private set; }

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

		#region Protected methods

		/// <summary>
		/// Create default write request options, 
		/// including the <see cref="EncryptionKey"/> if <paramref name="encrypt"/> is true.
		/// </summary>
		/// <param name="encrypt">If true, specify that the write operation must encrypt the data.</param>
		protected internal virtual BlobRequestOptions CreateDefaultWriteRequestOptions(bool encrypt)
		{
			var options = new BlobRequestOptions();

			if (encrypt)
			{
				options.EncryptionPolicy = CreateEncryptionPolicy();
			}

			return options;
		}

		/// <summary>
		/// Create default read request options, always including the <see cref="EncryptionKey"/>
		/// in case the file was encrypted.
		/// </summary>
		protected internal virtual BlobRequestOptions CreateDefaultReadRequestOptions()
		{
			return new BlobRequestOptions
			{
				EncryptionPolicy = CreateEncryptionPolicy()
			};
		}

		#endregion

		#region Private methods

		private BlobEncryptionPolicy CreateEncryptionPolicy()
		{
			return new BlobEncryptionPolicy(
				new SymmetricKey("SymmetricKey", this.EncryptionKey),
				null);
		}

		#endregion
	}
}
