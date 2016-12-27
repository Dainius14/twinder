using System;
using System.Collections.ObjectModel;
using Twinder.Model;
using Twinder.Model.Photos;

namespace Twinder.ViewModel.Design
{
	public class SampleMainVM : MainViewModel
	{
		private UpdatesModel _updates;
		public new UpdatesModel Updates
		{
			get { return _updates; }
			set { Set(ref _updates, value); }
		}

		private ObservableCollection<MatchModel> _matchList;
		public new ObservableCollection<MatchModel> MatchList
		{
			get { return _matchList; }
			set { Set(ref _matchList, value); }
		}



		public SampleMainVM()
		{
			Updates = new UpdatesModel();
			MatchList = new ObservableCollection<MatchModel>();

			FilterVM.ShowMoreFiltering = true;
			AuthStatus = AuthStatus.Okay;
			MatchesStatus = MatchesStatus.Okay;
			RecsStatus = RecsStatus.Okay;


			MatchModel Dolores = new MatchModel()
			{
				Id = "123",
				Person = new PersonModel()
				{
					Id = "123",
					Name = "Dolores",
					Bio = "Hi, I'm Dolores :p",
					Gender = 1,
					BirthDate = new DateTime(1987, 9, 7),
					Photos = new ObservableCollection<PhotoModel>()
					{
						new PhotoModel()
						{
							Url =  @"D:\Documents\Visual Studio 2015\Projects\twinder\SampleData\DoloresPics\dolores1.jpg",

							ProcessedFiles = new ObservableCollection<ProcessedFileModel>()
							{
								new ProcessedFileModel()
								{
									Url =  @"D:\Documents\Visual Studio 2015\Projects\twinder\SampleData\DoloresPics\dolores1.jpg"
								},
								new ProcessedFileModel()
								{
									Url =  @"D:\Documents\Visual Studio 2015\Projects\twinder\SampleData\DoloresPics\dolores1.jpg"
								},
								new ProcessedFileModel()
								{
									Url =  @"D:\Documents\Visual Studio 2015\Projects\twinder\SampleData\DoloresPics\dolores1.jpg"
								}
							}
						}
					}
				},
				Messages = new ObservableCollection<MessageModel>()
				{
					new MessageModel()
					{
						From = "123",
						SentDate = new DateTime(2016, 11, 10, 17, 5, 6),
						Message = "You're the best Dainius"
					}
				},
				MessageCount = 1,
				CreatedDate = new DateTime(2016, 11, 9, 0, 1, 2),
				LastActivityDate = new DateTime(2016, 11, 11, 2, 3, 4),
				IsSuperLike = true
			};

			RecList = new ObservableCollection<RecModel>();
			RecList.Add(new RecModel());

			MatchList.Add(Dolores);

		}
	}
}
