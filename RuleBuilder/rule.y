%{
#include <iostream>
#include <vector>
using namespace std;

#include "rule.flex.cpp"
#include "../syntax_tree.h"
void yyerror(const char* msg) {
  cerr << msg << endl;
}
%}

%union {
    int id;
	vector<int> *ids;
}


%token <id> NUMBER
%token <id> INVALID ID STRING IGNORED
%token <id> PROGRAM IS BEGINT END VAR TYPE PROCEDURE ARRAY RECORD
       IN OUT READ WRITE IF THEN ELSE ELSIF WHILE DO LOOP
       FOR EXIT RETURN TO BY AND OR NOT OF DIV MOD
       LPAREN  RPAREN LBRACKET RBRACKET LBRACE RBRACE COLON DOT
       SEMICOLON COMMA ASSIGN PLUS MINUS STAR SLASH BACKSLASH EQ
       NEQ LT LE GT GE LABRACKET RABRACKET 
%token <id> EOL

%type <id> exp
%type <id> factor
%type <id> term

%%
calc:
  | calc exp EOL { cout << "= " << $2 << endl; }
  ;
exp: factor
  | exp PLUS factor { $$ = helper_biop(PLUS,$1,$3); }
  | exp MINUS factor { $$ = helper_biop(MINUS,$1,$3); }
  ;
factor: term
  | factor STAR term { $$ = helper_biop(MULTIPLY,$1,$3);  }
  | factor SLASH term { $$ = helper_biop(DIVIDE,$1,$3); }
  ;
term: NUMBER
  | LPAREN exp RPAREN { $$ = helper_uniop(BRACKETS,$2); }
  ;
%%
