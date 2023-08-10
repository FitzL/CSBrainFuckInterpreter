using System;
using System.Text.RegularExpressions;
using System.IO;

namespace Brainfuck
{
	class Program
	{
		static void Main(String[] args)
		{
			byte[] tape = new byte[32768]; //define a tape to store values
			string input;
			if (args.Length > 0) 
			{
				input = File.ReadAllText(@args[0]); //read contents of file
			} 
			else
			{
				Console.Write("Give code: \n=> "); //ask user for code
				input = Console.ReadLine();	//read user input
			}
			char[] code = Parser(input); //parse comments
			Interpreter(code, tape); //run the interpreter
			Console.WriteLine("\nPress any key to close...");
			Console.ReadKey();
		}

		static char[] Parser(string _input)
		{
			Regex operators = new Regex(@"[\+\-\,\.\[\]\<\>]"); //valid operators, ignore the rest
			char[] input = _input.ToCharArray(); //take a string input and separate into characters
			string output = ""; //create an output string
			foreach (char i in input)
			{
				if (operators.IsMatch(Char.ToString(i))) //check every character for valid operations
				{
					output += i; //if valid, add to output
				}
			}
			output = (String.IsNullOrEmpty(output)) ? "++[>++++[>++++++++<-]<-]>>>++[<++++>-]<.<<+[>++++[>++++++++<-]<-]>>---.<++[>++++<-]>-..+++.>++[>++++[>++++++++<-]<-]>>>+++[<++++++++>-].<-.<<+[>+++[>++++++++<-]<-]>>.+++.<++[>---<-]>.<++[>----<-]>.>><[-]<[-]++[>++++[>++++<-]<-]>>+++++++++++++++++." : output; //give back actual operators in an array
			return output.ToCharArray();
		}

		static void Interpreter(char[] code, byte[] tape)
		{
			int p = 0; //tape pointer
			int i = 0; //code index
			while (i < code.Length)
			{
				p = p % tape.Length; //make sure you can't go out of bounds
				switch (code[i])
				{
					case '+': //increment
						tape[p]++;
						break;

					case '-':
						tape[p]--; //decrement
						break;

					case '>': //shift pointer position on the tape
						p++;
						break;

					case '<': //shift pointer position on the tape, backwards
						p--;
						break;

					case '.': //output cell content, in ascii
						Console.Write(Convert.ToChar(tape[p]));
						break;

					case ',': //read user key input
						Console.Write("request: =>");
						tape[p] = Convert.ToByte(Console.ReadKey().KeyChar);
						Console.WriteLine();
						break;
					case '[': //conditional jmp to, forwards
						if (tape[p] == 0)
						{
							i = jmptoClose(code, i);
						}
						break;
					case ']': //conditional jmp to, backwards
						if (tape[p] > 0)
						{
							i = jmptoOpen(code, i);
						}
						break;		
				}
				i++;
			}
		}
		static int jmptoClose(char[] code, int i) //logic for jmp to, forwards
		{
			i++;
			int depth = 1;
			while (depth > 0) //dealing with nesting
			{
				switch (code[i])
				{
					case '[':
						depth++;
						break;
					case ']':
						depth--;
						break;
				}
				i++;
			}
			return i; 
		}
		static int jmptoOpen(char[] code, int i) //logic for jmping back
		{
			i--;
			int depth = 1;
			while (depth > 0) //nesting thing
			{
				switch (code[i])
				{
					case '[':
						depth--;
						break;
					case ']':
						depth++;
						break;
				}
				i--;
			}
			return i; 			
		}
	}
}