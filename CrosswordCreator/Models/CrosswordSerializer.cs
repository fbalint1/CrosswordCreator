using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CrosswordCreator.Models
{
  internal static class CrosswordSerializer
  {
    internal static Crossword GetCrossword(string filename_)
    {
      if (!File.Exists(filename_))
      {
        return null;
      }

      var serializer = new XmlSerializer(typeof(Crossword));

      Crossword expenses = null;

      using (var stream = new FileStream(filename_, FileMode.Open))
      {
        expenses = (Crossword)serializer.Deserialize(stream);
      }

      return expenses;
    }

    internal static void PersistCrossword(Crossword crossword_, string workingDirectory_)
    {
      var finalPath = Path.Combine(workingDirectory_, crossword_.Goal.ToUpper() + ".xml");

      var serializer = new XmlSerializer(typeof(Crossword));

      using (var stream = new FileStream(finalPath, FileMode.Create, FileAccess.Write))
      {
        serializer.Serialize(stream, crossword_);
      }
    }
  }
}
