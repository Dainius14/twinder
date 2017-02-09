using System;
using Twinder.ViewModel;

namespace Twinder.SampleData
{
	public class SampleChatVM : ChatViewModel
	{
		public SampleChatVM()
		{
			Match = new Model.MatchModel()
			{
				Person = new Model.PersonModel()
				{
					Name = "Dolores",
					Id = "123"
				},
				Messages = new System.Collections.ObjectModel.ObservableCollection<Model.MessageModel>()
				{
					new Model.MessageModel()
					{
						Message = "Ayy lmao. Watcha doin fam?",
						SentDate = new DateTime(2017, 02, 09, 22, 19, 05),
						From = "123"
					},
					new Model.MessageModel()
					{
						Message = "Pls tex bak",
						SentDate = new DateTime(2017, 02, 09, 22, 19, 05),
						From = "123"
					},
					new Model.MessageModel()
					{
						Message = "Yeah for sure. Lorem ipsum dolor sit amet I actually don't remember more.",
						SentDate = new DateTime(2017, 02, 09, 22, 19, 05),
						From = "69"
					}

				}
			};
		}
	}
}
