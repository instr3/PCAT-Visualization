%{
#include "rule.tab.h"
#define MAX_STRING_BUFFER 255
#define MAX_ID_LEN 255

struct offset_t
{
	int line,col;
}current_offset={}, token_offset={};
char string_buffer[MAX_STRING_BUFFER+1]; int p_string_buffer;

void new_token(const char *type, char *token=0, int offset_line=-1, int offset_col=-1)
{
	if(!token)
		token=yytext;
	if(offset_col==-1)
	{
		offset_col=token_offset.col;
		offset_line=token_offset.line;
	}
	printf("[ New %s @ line %d, col %d%s%s ]\n", type, offset_line+1, offset_col+1, *token?": ":"", token);
}
void invalid_token(const char *text, char *token=0, int offset_line=-1, int offset_col=-1)
{
	if(!token)
		token=yytext;
	if(offset_col==-1)
	{
		offset_col=token_offset.col;
		offset_line=token_offset.line;
	}
	printf("[Error] %s @ line %d, col %d%s%s\n", text, offset_line+1, offset_col+1, *token?": ":"", token);

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

REVERSED	"AND"|"ARRAY"|"BEGIN"|"BY"|"DIV"|"DO"|"ELSE"|"ELSIF"|"END"|"EXIT"|"FOR"|"IF"|"IN"|"IS"|"LOOP"|"MOD"|"NOT"|"OF"|"OR"|"OUT"|"PROCEDURE"|"PROGRAM"|"READ"|"RECORD"|"RETURN"|"THEN"|"TO"|"TYPE"|"VAR"|"WHILE"|"WRITE"
letter		[a-zA-Z]
digit		[0-9]
operator	":="|"+"|"-"|"*"|"/"|"<"|"<="|">"|">="|"="|"<>"
delimiter	":"|";"|","|"."|"("|")"|"["|"]"|"{"|"}"|"[<"|">]"|"\\"
INTEGER		{digit}+
REAL		{digit}+"."{digit}*

ID			{letter}({letter}|{digit})*
WS			[ \t]+
%x SEG_STRING
%x SEG_COMMENT

%%
{WS}					/* skip blanks and tabs */
{operator}				new_token("OPERATOR");
{delimiter}				new_token("DELIMETER");
{REVERSED}				new_token("REVERSED");
{ID}					if(yyleng>MAX_ID_LEN) invalid_token("TOO LONG ID"); else new_token("ID");
{INTEGER}				if(yyleng>10||strtoll(yytext,0,10)>2147483647)invalid_token("TOO BIG INTEGER"); else new_token("NUMBER");
{REAL}					new_token("NUMBER");
\"						p_string_buffer=0,BEGIN(SEG_STRING);
<SEG_STRING>\"			if(p_string_buffer<=MAX_STRING_BUFFER) string_buffer[p_string_buffer]=0,new_token("STRING", string_buffer),BEGIN(INITIAL); else invalid_token("TOO LONG STRING"),BEGIN(INITIAL);
<SEG_STRING>\n			|
<SEG_STRING><<EOF>>		string_buffer[p_string_buffer]=0,invalid_token("UNBALANCED QUOTES", string_buffer),BEGIN(INITIAL);
<SEG_STRING>[[:print:]]	if(p_string_buffer<MAX_STRING_BUFFER+1) string_buffer[p_string_buffer++]=yytext[0];
<SEG_STRING>.			invalid_token("INVALID CHARACTER IN STRING", string_buffer); /* if control characters appears in string, throw errors */
"(*"					BEGIN(SEG_COMMENT),new_token("COMMENT BLOCK","");
<SEG_COMMENT>"*)"		BEGIN(INITIAL);
<SEG_COMMENT><<EOF>>	invalid_token("UNTERMINATED COMMENT"),BEGIN(INITIAL);
<SEG_COMMENT>.			/* skip content in comment */
<INITIAL,SEG_COMMENT>\n	/* skip \n */
.						invalid_token("UNIDENTIFIED CHARACTER");
%%
