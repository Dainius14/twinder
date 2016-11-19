using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twinder.Helpers
{
	/// <summary>
	/// Specialized collection, which fires property changed event, when collection changes
	/// </summary>
	class UpdatingCollection<T> : ObservableCollection<T>, INotifyPropertyChanged
	{
		public UpdatingCollection() : base()
		{
			CollectionChanged += UpdatingCollection_CollectionChanged;
		}

		private void UpdatingCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
				foreach (var item in e.NewItems)
				{
					(item as INotifyPropertyChanged).PropertyChanged += UpdatingCollection_PropertyChanged;
				}
		}

		private void UpdatingCollection_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			throw new NotImplementedException();
		}

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);
		}
	}
}
