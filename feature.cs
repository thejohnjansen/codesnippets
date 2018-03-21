
static bool CompareStrings(string s1, string s2)
{
  foreach (char c in s1)
  {
     int ix = s2.IndexOf(c);
     if (ix == -1)
       return false;
  }
  return true;
}