%{
#include <iostream>
using namespace std;

#include "rule.flex.cpp"
void yyerror(const char* msg) {
  cerr << msg << endl;
}
%}

%union {
    double val;
}

%token <val> NUMBER
%token ADD SUB MUL DIV OP CP
%token EOL

%type <val> exp
%type <val> factor
%type <val> term

%%
calc:
  | calc exp EOL { cout << "= " << $2 << endl; }
  ;
exp: factor
  | exp ADD factor { $$ = $1 + $3; }
  | exp SUB factor { $$ = $1 - $3; }
  ;
factor: term
  | factor MUL term { $$ = $1 * $3; }
  | factor DIV term { $$ = $1 / $3; }
  ;
term: NUMBER
  | OP exp CP { $$ = $2; }
  ;
%%
