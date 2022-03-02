using System;
using System.Linq;
using System.Collections.Generic;

//Note: the theory and method behind this is mostly taken from:
//  https://rpg.stackexchange.com/questions/70802/how-can-i-test-whether-a-die-is-fair

namespace PracticeSpace {

  public class DieTest {

    // Creates a dictionary of the probability values for the different number of faces
    Dictionary<int, double[]> probabilityValues = new Dictionary<int, double[]>();

    private void initProbabilityValues() {
      this.probabilityValues.Add(3, new double[] {4.6, 5.9, 7.3, 9.2});
      this.probabilityValues.Add(4, new double[] {6.2, 7.8, 9.3, 11.3});
      this.probabilityValues.Add(6, new double[] {9.2, 11.0, 12.8, 15.1});
      this.probabilityValues.Add(8, new double[] {12.1, 14.1, 16.1, 18.4});
      this.probabilityValues.Add(10, new double[] {14.6, 16.9, 19.1, 21.6});
      this.probabilityValues.Add(12, new double[] {17.2, 19.6, 21.9, 24.7});
      this.probabilityValues.Add(20, new double[] {27.2, 30.1, 32.8, 36.1});
    }

    int numFaces = 0;
    int[] rolls;
    int rollCount = 1;

    private T gatherInput<T>(string prompt){
      Console.Write(prompt);
      string input = Console.ReadLine();

      Console.WriteLine("");
      return (T)Convert.ChangeType(input, typeof(T));
    }

    private void ClearLastLine() {
      Console.SetCursorPosition(0, Console.CursorTop - 1);
      Console.Write(new string(' ', Console.BufferWidth));
      Console.SetCursorPosition(0, Console.CursorTop - 1);
    }

    // gather roll doesn't use gather input so that you don't have to hit enter every time
    private int gatherRoll(int count) {
      Console.Write($"Roll {count}: ");
      ConsoleKeyInfo key;
      key = Console.ReadKey();
      int returnNum = int.Parse(key.KeyChar.ToString());
      // this is so you don't have to hit enter for a 2-digit number
      if(this.numFaces >= 10) {
        key = Console.ReadKey();
        returnNum = (returnNum*10) + int.Parse(key.KeyChar.ToString());
      }
      Console.WriteLine("");
      return returnNum;
    }

    private void gatherRolls() {
      int roll = 0;
      while(roll <= this.numFaces) {
        try {
          roll = this.gatherRoll(this.rollCount);
          if(roll <= this.numFaces) {
            this.rolls[roll-1] += 1;
            this.rollCount++;
            this.ClearLastLine();
          }
        } catch {
          this.ClearLastLine();
          Console.Write("Not a valid input! Try again: ");
          roll = 0;
        }
      }
    }

    private string buildGraphLine(int value) {
      string display = "|  ";
      for(int i = 0; i<this.numFaces; i++) {
        if(this.rolls[i] >= value) {
          display += "*";
        } else {
          display += " ";
        }
        display += "  |  ";
      }
      return display;
    }

    private string buildBreakLine() {
      string display = "|";
      for(int i = 0; i<this.numFaces; i++) {
        display += "-----|";
      }
      return display;
    }

    private string buildDieLine() {
      string display = "|";
      for(int i = 1; i<=this.numFaces; i++) {
        display += String.Format("{0,5}|", i);
      }
      return display;
    }

    private void displayDistribution(double[] xk2s) {
      int maxValue = this.rolls.Max();
      // Display the graph
      for(int checkVal = maxValue; checkVal>0; checkVal--) {
        Console.WriteLine(this.buildGraphLine(checkVal));
      }

      string breakLine = this.buildBreakLine();
      Console.WriteLine(breakLine);
      Console.WriteLine(this.buildDieLine());
      Console.WriteLine(breakLine);

      // Display xk2s
      Console.Write("|");
      for(int i = 0; i < this.numFaces; i++) {
        Console.Write("{0,5:0.00}|", xk2s[i]);
      }
    }

    private double[] calculateXk2s() {
      int expected = this.rollCount/this.numFaces;
      double[] xk2s = new double[this.numFaces];
      for(int i = 0; i<this.numFaces; i++) {
        xk2s[i] = Math.Pow((this.rolls[i]-expected), 2)/expected;
      }
      return xk2s;
    }

    private void displayResults(double[] xk2s) {
      double x2 = xk2s.Sum();

      Console.WriteLine("Here is the distribution for this die:");
      this.displayDistribution(xk2s);

      Console.Write("\n\n");
      Console.WriteLine($"Overall x2 was calculated to be: {x2}\n\n");
      Console.WriteLine("How to interpret this data:");
      Console.WriteLine("The number below the die in the graph is the deviance from the expected number of times it should show up. The closer to 0 the better.\n");
      Console.WriteLine("Given a fair die:");
      Console.WriteLine($"  x2             < {probabilityValues[this.numFaces][0]}   90% of the time.");
      Console.WriteLine($"  x2 > {probabilityValues[this.numFaces][0]}   and < {probabilityValues[this.numFaces][1]} 95% of the time.");
      Console.WriteLine($"  x2 > {probabilityValues[this.numFaces][1]} and < {probabilityValues[this.numFaces][2]} 97% of the time.");
      Console.WriteLine($"  x2 > {probabilityValues[this.numFaces][2]} and < {probabilityValues[this.numFaces][3]} 99% of the time.");
      Console.WriteLine("Meaning:");
      Console.WriteLine($"  x2 < {probabilityValues[this.numFaces][0]} -> No concern");
      Console.WriteLine($"  x2 between {probabilityValues[this.numFaces][0]} and {probabilityValues[this.numFaces][2]} -> Maybe concern?");
      Console.WriteLine($"  x2 over {probabilityValues[this.numFaces][2]} -> Probably unfair.");
      Console.WriteLine("You can choose where to draw the line, it just depends on how confident you want to be");
    }

    public void runProgram() {
      this.initProbabilityValues();

      Console.WriteLine("This will calculate the probability that a 3, 4, 6, 8, 10, 12, or 20 -sided die is unfair.");
      this.numFaces = gatherInput<int>("How many faces does the die you want to test have?: ");
      this.rolls = new int[this.numFaces];

      Console.WriteLine("How many times do I need to roll?");
      Console.WriteLine("  Depends on how confident you want to be.");
      Console.WriteLine($"  {this.numFaces*5} rolls -> ~20% chance of catching unfair die");
      Console.WriteLine($"  {this.numFaces*10}-> ~70% chance of catching unfair die");
      Console.WriteLine($"  {this.numFaces*15} rolls -> ~90% chance of catching unfair die");
      Console.WriteLine($"  {this.numFaces*20} -> ~99% chance of catching unfair die");
      Console.WriteLine($"  Also, doing a multiple of {this.numFaces} will help give the cleanest results.");
      Console.WriteLine($"The program will tell you what roll you are on. Enter a number larger than {this.numFaces} when you want to end.\n\n");
      if(this.numFaces >= 10) {
        Console.WriteLine("For numbers less than 10, enter with a leading 0. e.g. 03, 04, 08");
      }


      char contResp = 'y';

      while (contResp == 'y') {
        this.gatherRolls();
        Console.WriteLine("Thank you!\n\n");

        Console.WriteLine("Here are the results:");

        this.displayResults(this.calculateXk2s());

        contResp = gatherInput<char>("Would you like to roll this die some more?(Y/N): ");
        contResp = Char.ToLower(contResp);
      }
    }
  }

  class Start {
    static void Main(string[] args) {
      DieTest dt = new DieTest();

      dt.runProgram();
    }
  }
}