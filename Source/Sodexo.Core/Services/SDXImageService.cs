using System;
using System.Threading.Tasks;
using Sodexo.RetailActivation.Portable.Models;
using Newtonsoft.Json;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Sodexo.RetailActivation.Portable.Helpers;

namespace Sodexo.Core
{
	public class SDXImageService : SDXBaseService
	{
		private string BaseUrl = Constants.APIRootUrl + "/photoposting/";

		public SDXImageService () : base ()
		{

		}

		protected async Task<PhotoPostingSasRequest> DoGetPhotoPostingToken (string fileName, string modelTypeName, string id)
		{
			var uri = BaseUrl + fileName + "/" + modelTypeName + "/" + id + "?subscription-key=" + Constants.APISubscriptionKey;

			if (modelTypeName == "Promotion" || modelTypeName == "OfferResponse")
				uri += "&isActivationPhoto=" + true.ToString ();

			var responseBody = await GetAsync (uri);

			if (IsSuccessed) {
				Console.WriteLine ("GetPhotoPostingToken Success:");
				WriteResponseToConsole (responseBody);
				return JsonConvert.DeserializeObject<PhotoPostingSasRequest> (responseBody);
			} else {
				Console.WriteLine ("GetPhotoPostingToken Failed: " + ErrorMessage);
				return null;
			}
		}

		protected async Task<string> DoUploadPhoto (PhotoPostingSasRequest request, Byte[] bytes)
		{
			var uri = request.SasUrl + request.SasToken;

			HttpContent content = new ByteArrayContent(bytes);
			content.Headers.Add("Content-Type", MimeMappingHelper.GetMimeMapping(request.FileName));

			await PutAsync (uri, content);

			if (IsSuccessed) {
				Console.WriteLine ("UploadPhoto Success:");
				return "Success";
			} else {
				Console.WriteLine ("UploadPhoto Failed: " + ErrorMessage);
				return "Failed";
			}
		}

		protected async Task <PhotoModel> DoAddPhotoToExistingItem (PhotoPostingSasRequest request, string fileName)
		{
			var uri = BaseUrl + fileName + "/" + request.ModelTypeName + "/" + request.Id + "?subscription-key=" + Constants.APISubscriptionKey;

			if (request.ModelTypeName == "Promotion" || request.ModelTypeName == "OfferResponse")
				uri += "&isActivationPhoto=" + true.ToString ();

			var serializedObjStr = JsonConvert.SerializeObject (request);

			HttpContent content = new StringContent (serializedObjStr, System.Text.Encoding.UTF8, "application/json");

			var responseBody = await PostAsync (uri, content);

			if (IsSuccessed) {
				Console.WriteLine ("AddPhotoToExistingItem Success:");
				WriteResponseToConsole (responseBody);
				return PhotoModel.Get (responseBody);
			} else {
				Console.WriteLine ("AddPhotoToExistingItem Failed: " + ErrorMessage);
				return null;
			}
		}
	}
}

