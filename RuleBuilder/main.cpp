#include <iostream>
#include <stdio.h>
#include "syntax_tree.h"
using namespace std;

extern int yyparse();
extern int yylex();
extern FILE* yyin;

int main(int argc, char* args[]) {
	if (argc>1) {
		FILE *file = fopen(args[1], "r");
		if (!file) {
			cerr << "Can't open file" << endl;
			return 1;
		}
		else {
			yyin = file;
		}
	}
	bool as_interface = false;
	bool as_tree = false;
	if (argc > 2)
	{
		if (strcmp(args[2], "-i") == 0)
			as_interface = true;
		else if (strcmp(args[2], "-t") == 0)
			as_tree = true;
	}
	// FILE *file = fopen("C:\\Users\\jjy\\Documents\\2jjy\\Programming\\CSharp\\TejiLang-Toy\\tutorial\\Compiler_Project-master\\tests\\test08.pcat", "r");
	// yyin = file;
	if (0 == yyparse())
	{
		if(as_interface)
			output_syntax_tree();
		else if(as_tree)
			visualize_syntax_tree();
		else
		{
			printf("Compile Successful\n");
			visualize_syntax_tree();
		}
	}
	// freopen("C:\\Users\\jjy\\Documents\\2jjy\\Programming\\CSharp\\TejiLang-Toy\\Debug\\test17-space.out", "w",stdout);
	// output_syntax_tree();
	return 0;
}