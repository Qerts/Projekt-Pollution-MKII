using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Windows.UI.Xaml.Media;
using Windows.UI;


namespace Pollution.ViewModels
{
    public static class Utils
    {


        public static void Sort<TSource, TKey>(this Collection<TSource> source, Func<TSource, TKey> keySelector)
        {
            List<TSource> sortedList = source.OrderBy(keySelector).ToList();
            source.Clear();
            foreach (var sortedItem in sortedList)
                source.Add(sortedItem);
        }

        public static void SortDesc<TSource, TKey>(this Collection<TSource> source, Func<TSource, TKey> keySelector)
        {
            List<TSource> sortedList = source.OrderByDescending(keySelector).ToList();
            source.Clear();
            foreach (var sortedItem in sortedList)
                source.Add(sortedItem);
        }

        public static void Sort2Levels<TSource, TKey>(this Collection<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TKey> key2Selector)
        {
            List<TSource> sortedList = source.OrderBy(keySelector).ThenBy(key2Selector).ToList();
            source.Clear();
            foreach (var sortedItem in sortedList)
                source.Add(sortedItem);
        }

        public static void SortDesc2Levels<TSource, TKey>(this Collection<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TKey> key2Selector)
        {
            List<TSource> sortedList = source.OrderByDescending(keySelector).ThenBy(key2Selector).ToList();
            source.Clear();
            foreach (var sortedItem in sortedList)
                source.Add(sortedItem);
        }

        public static SolidColorBrush GetStateColorBrush(int value)
        {
            if (value == 1)
                return new SolidColorBrush(Color.FromArgb(255, 0, 138, 0));

            if (value == 2)
                return new SolidColorBrush(Color.FromArgb(255, 132, 161, 47));

            if (value == 3)
                return new SolidColorBrush(Color.FromArgb(255, 184, 177, 0));

            if (value == 4)
                return new SolidColorBrush(Color.FromArgb(255, 207, 141, 19));

            if (value == 5)
                return new SolidColorBrush(Color.FromArgb(255, 207, 84, 23));

            if (value == 6)
                return new SolidColorBrush(Color.FromArgb(255, 207, 51, 27));

            if (value == 7)
                return new SolidColorBrush(Color.FromArgb(255, 143, 143, 143));

            if (value == 8)
                return new SolidColorBrush(Color.FromArgb(255, 138, 136, 156));

            return new SolidColorBrush(Colors.Black);
        }
        
    }
}
