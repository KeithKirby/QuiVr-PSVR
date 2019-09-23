using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
public class BitCompression : MonoBehaviour {

    public static Dictionary<char, string> Backward = new Dictionary<char, string>(){
        {'a', "1"}, {'b', "0"}, {'c', "10"}, {'d', "11"}, {'e', "01"}, {'f', "00"},
        {'g', "100"},{'h', "101"},{'i', "110"},{'j', "111"},{'k', "001"},{'l', "010"},
        {'m', "000"},{'n', "0000"},{'o', "0001"},{'p', "0010"},{'q', "0100"},{'r', "1000"},
        {'s', "1001"},{'t', "1010"},{'u', "1011"},{'v', "1100"},{'w', "1101"},{'x', "1110"},
        {'y', "1111"},{'z', "00001"},{'A', "00010"},{'B', "00011"},{'C', "00100"},{'D', "00101"},
        {'E', "00110"},{'F', "00111"},{'G', "01000"},{'H', "01001"},{'I', "01010"},{'J', "01011"},
        {'K', "01100"},{'L', "01101"},{'M', "01110"},{'N', "01111"},{'O', "10000"},{'P', "10001"},
        {'R', "10010"},{'S', "10011"},{'T', "10101"},{'U', "10110"},{'V', "10111"},{'Q', "10100"},
        {'W', "11000"},{'X', "11001"},{'Y', "11010"},{'Z', "11011"},{'1', "11100"},{'2', "11101"},
        {'3', "00000"},{'4', "11110"},{'5', "11111"}
    };

    public static string Encode(string val)
    {
        string x = "";
        List<string> groups = (from Match m in Regex.Matches(val, @"\d{1,5}")
                               select m.Value).ToList();
        Dictionary<string, char> f = Forward();
        foreach(var v in groups)
        {
            if(f.ContainsKey(v))
                x += f[v];
        }
        return x;
    }

    public static string Decode(string val)
    {
        string x = "";
        foreach (var v in val)
        {
            if (Backward.ContainsKey(v))
                x += Backward[v];
        }
        return x;
    }

    public static Dictionary<string, char> Forward()
    {
        Dictionary<string, char> f = new Dictionary<string, char>();
        foreach(var k in Backward.Keys)
        {
            f.Add(Backward[k], k);
        }
        return f;
    }

}
