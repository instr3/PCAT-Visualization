%{
#include "rule.tab.h"
%}
%option     nounput
%option     noyywrap

DIGIT       [0-9]
INTEGER     {DIGIT}+
REAL        {DIGIT}+"."{DIGIT}*
WS          [ \t]+

%%
{WS}        /* skip blanks and tabs */
"+"         return ADD;
"-"         return SUB;
"*"         return MUL;
"/"         return DIV;
"("         return OP;
")"         return CP;
\n          return EOL;
{INTEGER}|{REAL}   { yylval.val = atof(yytext); return NUMBER; }
%%
