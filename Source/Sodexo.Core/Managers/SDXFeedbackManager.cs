using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Core
{
	public class SDXFeedbackManager : SDXFeedbackService
	{
		public SDXFeedbackManager () : base ()
		{

		}

		public async Task <FeedbackModel> AddFeedback (int feedbackTypeId, int starRatingCount, string comment, int modelId = 0, bool isResponseRequested = false)
		{
			return await DoAddFeedback (feedbackTypeId, starRatingCount, comment, modelId, isResponseRequested);
		}
	}
}

