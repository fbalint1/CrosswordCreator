using System;

namespace CrosswordCreator.Utilities
{
  [AttributeUsage(AttributeTargets.Field)]
  internal class IconAttribute : Attribute
  {
    private readonly string _iconPath;

    public string IconPath => _iconPath;

    public IconAttribute(string iconPath_)
    {
      _iconPath = iconPath_;
    }
  }
}
