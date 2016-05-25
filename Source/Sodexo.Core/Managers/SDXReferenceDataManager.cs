using System;
using System.Threading.Tasks;
using Sodexo.RetailActivation.Portable.Models;
using System.Collections.Generic;

namespace Sodexo.Core
{
	public class SDXReferenceDataManager : SDXReferenceDataService
	{
		public ReferenceDataModel DataModel = null;

		public Dictionary <int, DecisionTreeModel> DecisionTrees = new Dictionary<int, DecisionTreeModel> ();

		public SDXReferenceDataManager ()
		{

		}

		public async Task<ReferenceDataModel> LoadReferenceData (bool isIncludeConsumerTypes = true, bool isIncludeAnnualSalesRanges = true, bool isIncludeConsumersOnSiteRanges = true, bool isIncludeLocalCompetitionTypes = true, bool isIncludeOfferCategories = true, bool isIncludeFeedbackTypes = true)
		{
			DataModel = await GetReferenceData (isIncludeConsumerTypes, isIncludeAnnualSalesRanges, isIncludeConsumersOnSiteRanges, isIncludeLocalCompetitionTypes, isIncludeOfferCategories, isIncludeFeedbackTypes);

			return DataModel;
		}

		public async Task <DecisionTreeModel> LoadDecisionTree (int offerCategoryId, bool isIncludeVersions = true, bool isIncludeNodes = true, bool isIncludeOnlyCurrentVersion = true)
		{
			DecisionTreeModel tree = await DoLoadDecisionTree (offerCategoryId, isIncludeVersions, isIncludeNodes, isIncludeOnlyCurrentVersion);

			if (tree != null)
				DecisionTrees [offerCategoryId] = tree;

			return tree;
		}
	}
}

