using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using Pollution.Resources;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Pollution.Resources
{
    /// <summary>
    /// Třída zajišťující přístup k jednotlivým položkám zdrojů AppResources
    /// </summary>
    public class Resources : INotifyPropertyChanged
    {
        public Resources()
        {
        }

        //private static AppResources localizedResources = new AppResources();

        /// <summary>
        /// Property zajišťující přístup ke zdrojům
        /// </summary>
        //public AppResources LocalizedResources { get { return localizedResources; } }

        /// <summary>
        /// Metoda, která umožňuje informovat všechny napojené části o změně zdrojů.
        /// Využívá se pro dynamickou změnu zdrojů/jazyka v době běhu programu.
        /// </summary>
        public void ResetResources()
	    {
	       // OnPropertyChanged(() => LocalizedResources);
	    }

        /// <summary>
        /// Metody a událost potřebná pro korektní propojení zdrojů a jejich změnu při běhu programu
        /// </summary>
         #region INotifyPropertyChanged region
         public event PropertyChangedEventHandler PropertyChanged;

         public void OnPropertyChanged<T>(Expression<Func<T>> selector)
         {
             if (PropertyChanged != null)
             {
                 PropertyChanged(this, new PropertyChangedEventArgs(GetPropertyNameFromExpression(selector)));
             }
         }

         public static string GetPropertyNameFromExpression<T>(Expression<Func<T>> property)
         {
             var lambda = (LambdaExpression)property;
             MemberExpression memberExpression;

             if (lambda.Body is UnaryExpression)
             {
                 var unaryExpression = (UnaryExpression)lambda.Body;
                 memberExpression = (MemberExpression)unaryExpression.Operand;
             }
             else
             {
                 memberExpression = (MemberExpression)lambda.Body;
             }

             return memberExpression.Member.Name;
         }
         #endregion
    }
}
