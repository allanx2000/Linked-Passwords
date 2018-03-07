using Innouvous.Utils.Merged45.MVVM45;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LinkedPasswords.Models
{
    public class Entry
    {

        public int ID { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public int? PasswordId { get; set; }


        /** ViewModel **/

        private string description;
        public string Description {
            get { return description; }
            set
            {
                description = value;

                if (description != null)
                {
                    description = description.Trim();
                    if (description.Length == 0)
                        description = null;
                }
            }
        }

        public SolidColorBrush TextColor
        {
            get { return PasswordId == null ? BrushColors.Red : BrushColors.Black; }
        }


    }
}
