using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twinder.Models.Updates;

namespace Twinder.ViewModel
{
	public class MatchProfileViewModel : ViewModelBase
	{
		private MatchModel _match;
		public MatchModel Match
		{
			get { return _match; }
			set
			{
				Set(ref _match, value);
			}
		}

	}
}
