using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NReadability
{
  public static class EnumerableExtensions
  {
      // I have no idea how the original templatized version of this could have ever worked.
      // This is the correct implementation for the case of XElement. I need to examine the tests that use the 
      // templatized version fo this.
      public static XElement SingleOrNone(this IEnumerable<XNode> enumerable)
      {
          if (enumerable == null)
          {
              throw new ArgumentNullException("enumerable");
          }

          XElement result = null;
          foreach (XNode node in enumerable)
          {
              var element = node as XElement;
              if (element != null)
              {
                  if (result == null)
                  {
                      result = element;
                  }
                  else
                  {
                      return null;
                  }
              }
          }
          return result;
      }

    /// <summary>
    /// Returns the only one element in the sequence or default(T) if either the sequence doesn't contain any elements or it contains more than one element.
    /// </summary>
    public static T SingleOrNone<T>(this IEnumerable<T> enumerable)
      where T : class
    {
      // ReSharper disable PossibleMultipleEnumeration

      if (enumerable == null)
      {
        throw new ArgumentNullException("enumerable");
      }

      T firstElement = enumerable.FirstOrDefault<T>();

      if (firstElement == null)
      {
        // no elements
        return null;
      }

      T secondElement = enumerable.Skip(1).FirstOrDefault();

      if (secondElement != null)
      {
        // more than one element
        return null;
      }

      return firstElement;

      // ReSharper restore PossibleMultipleEnumeration
    }
  }
}
