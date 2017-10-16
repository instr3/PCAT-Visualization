#include <iostream>
#include <stdio.h>
using namespace std;

int yyparse();
extern int yylex();
extern FILE* yyin;

int main(int argc, char* args[]) {
	if (true) {
		FILE *file = fopen("test.txt", "r");
		if (!file) {
			cerr << "Can't open file" << endl;
			return 1;
		}
		else {
			yyin = file;
		}
	}
	// yyparse();
	yylex();
	return 0;
}