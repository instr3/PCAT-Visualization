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
	FILE *file = fopen("C:\\Users\\jjy\\Documents\\2jjy\\Programming\\CSharp\\TejiLang-Toy\\tutorial\\Compiler_Project-master\\tests\\test17-space.pcat", "r");
	yyin = file;
	printf("%d\n", yyparse());
	visualize_syntax_tree();
	freopen("C:\\Users\\jjy\\Documents\\2jjy\\Programming\\CSharp\\TejiLang-Toy\\Debug\\test17-space.out", "w",stdout);
	
	output_syntax_tree();
	while (1);
	return 0;
}