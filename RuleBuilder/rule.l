%{
#include "rule.tab.h"
struct offset_t
{
	int line,col;
}current_offset={}, token_offset={};

void new_token(const char *type, char *token=0, int offset_line=-1, int offset_col=-1)
{
	if(!token)
		token=yytext;
	if(offset_col==-1)
	{
		offset_col=token_offset.col;
		offset_line=token_offset.line;
	}
	printf("[ New %s @ line %d, col %d: %s ]\n",type, offset_line+1, offset_col+1, token);
}
#define YY_USER_ACTION do\
{\
	token_offset=current_offset;\
	for(int i=0;i<yyleng;++i)\
	{\
		if(yytext[i]=='\n')\
		{\
			current_offset.line++;\
			current_offset.col=0;\
		}\
		else current_offset.col++;\
	}\
}while(0);
%}
%option     nounput
%option     noyywrap

DIGIT       [0-9]
INTEGER     {DIGIT}+
REAL        {DIGIT}+"."{DIGIT}*
WS          [ \t]+

%%
{WS}        /* skip blanks and tabs */
"+"         new_token("add");
"-"         new_token("sub");
"*"         new_token("mul");
"/"         new_token("div");
"("         new_token("lb");
")"         new_token("rb");
\n          new_token("eol");
{INTEGER}|{REAL}   new_token("number");
%%
