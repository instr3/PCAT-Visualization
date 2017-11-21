#include <iostream>
#include <stdio.h>
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
	while(1)
		yyparse();
	system("pause");
	return 0;
}