using System;
using System.Threading.Tasks;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Core
{
	public class SDXImageManager : SDXImageService
	{
		public SDXImageManager () : base ()
		{

		}

		public async Task <PhotoPostingSasRequest> GetPhotoPostingToken (string fileName, string modelTypeName, string id)
		{
			return await DoGetPhotoPostingToken (fileName, modelTypeName, id);
		}

		public async Task <string> UploadPhoto (PhotoPostingSasRequest request, Byte[] bytes)
		{
			return await DoUploadPhoto (request, bytes);
		}

		public async Task <PhotoModel> AddPhotoToExistingItem (PhotoPostingSasRequest request, string fileName)
		{
			return await DoAddPhotoToExistingItem (request, fileName);
		}
	}
}

