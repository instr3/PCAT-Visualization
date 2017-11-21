%{
#include "rule.tab.h"
#include "../syntax_tree.h"
#define MAX_STRING_BUFFER 255
#define MAX_ID_LEN 255

struct offset_t
{
	int line,col;
}current_offset={}, token_offset={};
char string_buffer[MAX_STRING_BUFFER+2]; int p_string_buffer;
char error_info_buffer[1024];

#define USER_BISON

#ifdef USER_BISON // Bison mode
#define commit_token(type_macro, ...) (new_token(type_macro, #type_macro, ##__VA_ARGS__),type_macro)
#define YIELD return ((void *)0), 
#else // Flex mode
#define commit_token(type_macro, ...) new_token(type_macro, #type_macro, ##__VA_ARGS__)
#define YIELD ((void *)0), 
#endif
#define TT return NEWT;

void new_token(int type_macro, const char *type, char *token=0, int offset_line=-1, int offset_col=-1)
{
	if(!token)
		token=yytext;
	if(offset_col==-1)
		offset_col=token_offset.col;
	if(offset_line==-1)
		offset_line=token_offset.line;
#ifndef USER_BISON
	printf("[New %s] @ line %d, col %d%s%s\n", type, offset_line+1, offset_col+1, *token?": ":"", token);
#else
	if(type_macro!=IGNORED)
		yylval.id = create_node(type_macro, type, offset_line, offset_col, strlen(yytext), 0);
#endif
}
int invalid_token(const char *text, char *token=0, int offset_line=-1, int offset_col=-1)
{
	if(!token)
		token=yytext;
	if(offset_col==-1)
		offset_col=token_offset.col;
	if(offset_line==-1)
		offset_line=token_offset.line;
	printf("[Error: %s] @ line %d, col %d%s%s\n", text, offset_line+1, offset_col+1, *token?": ":"", token);
	return INVALID; // Uncommited token
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

RESERVED	"AND"|"ARRAY"|"BEGIN"|"BY"|"DIV"|"DO"|"ELSE"|"ELSIF"|"END"|"EXIT"|"FOR"|"IF"|"IN"|"IS"|"LOOP"|"MOD"|"NOT"|"OF"|"OR"|"OUT"|"PROCEDURE"|"PROGRAM"|"READ"|"RECORD"|"YIELD"|"THEN"|"TO"|"TYPE"|"VAR"|"WHILE"|"WRITE"
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
"PROGRAM"   			YIELD commit_token(PROGRAM);
"IS"        			YIELD commit_token(IS);
"BEGIN"     			YIELD commit_token(BEGINT);
"END"       			YIELD commit_token(END);
"VAR"       			YIELD commit_token(VAR);
"TYPE"      			YIELD commit_token(TYPE);
"PROCEDURE" 			YIELD commit_token(PROCEDURE);
"ARRAY"     			YIELD commit_token(ARRAY);
"RECORD"    			YIELD commit_token(RECORD);
"IN"        			YIELD commit_token(IN);
"OUT"       			YIELD commit_token(OUT);
"READ"      			YIELD commit_token(READ);
"WRITE"     			YIELD commit_token(WRITE);
"IF"        			YIELD commit_token(IF);
"THEN"      			YIELD commit_token(THEN);
"ELSE"      			YIELD commit_token(ELSE);
"ELSIF"     			YIELD commit_token(ELSIF);
"WHILE"     			YIELD commit_token(WHILE);
"DO"        			YIELD commit_token(DO);
"LOOP"      			YIELD commit_token(LOOP);
"FOR"       			YIELD commit_token(FOR);
"EXIT"      			YIELD commit_token(EXIT);
"RETURN"    			YIELD commit_token(RETURN);
"TO"        			YIELD commit_token(TO);
"BY"        			YIELD commit_token(BY);
"AND"       			YIELD commit_token(AND);
"OR"        			YIELD commit_token(OR);
"NOT"       			YIELD commit_token(NOT);
"OF"        			YIELD commit_token(OF);
"DIV"       			YIELD commit_token(DIV);
"MOD"       			YIELD commit_token(MOD);
"("						YIELD commit_token(LPAREN);
")"						YIELD commit_token(RPAREN);
"["						YIELD commit_token(LBRACKET);
"]"						YIELD commit_token(RBRACKET);
"{"						YIELD commit_token(LBRACE);
"}"						YIELD commit_token(RBRACE);
":"						YIELD commit_token(COLON);
"."						YIELD commit_token(DOT);
";"						YIELD commit_token(SEMICOLON);
","						YIELD commit_token(COMMA);
":="					YIELD commit_token(ASSIGN);
"+"						YIELD commit_token(PLUS);
"-"						YIELD commit_token(MINUS);
"*"						YIELD commit_token(STAR);
"/"						YIELD commit_token(SLASH);
"\\"					YIELD commit_token(BACKSLASH);
"="						YIELD commit_token(EQ);
"<>"					YIELD commit_token(NEQ);
"<"						YIELD commit_token(LT);
"<="					YIELD commit_token(LE);
">"						YIELD commit_token(GT);
">="					YIELD commit_token(GE);
"[<"					YIELD commit_token(LABRACKET);
">]"					YIELD commit_token(RABRACKET); 
{ID}					YIELD yyleng>MAX_ID_LEN ? invalid_token("TOO LONG ID") : commit_token(ID);
{INTEGER}				YIELD strtoll(yytext,0,10)>2147483647 ? invalid_token("TOO BIG INTEGER"): commit_token(NUMBER);
{REAL}					YIELD commit_token(NUMBER);
\"						p_string_buffer=0,BEGIN(SEG_STRING);
<SEG_STRING>\"			YIELD string_buffer[p_string_buffer]=0,BEGIN(INITIAL),p_string_buffer<=MAX_STRING_BUFFER?(token_offset.col-=p_string_buffer+1,commit_token(STRING, string_buffer)):invalid_token("TOO LONG STRING", string_buffer);
<SEG_STRING>\n			|
<SEG_STRING><<EOF>>		YIELD string_buffer[p_string_buffer]=0,BEGIN(INITIAL),invalid_token("UNBALANCED QUOTES", string_buffer);
<SEG_STRING>[[:print:]]	if(p_string_buffer<MAX_STRING_BUFFER+1) string_buffer[p_string_buffer++]=yytext[0];
<SEG_STRING>.			YIELD sprintf(error_info_buffer,"\\x%02x",*yytext),invalid_token("INVALID CHARACTER IN STRING",error_info_buffer); /* if control characters appears in string, throw errors */
"(*"					BEGIN(SEG_COMMENT),new_token(IGNORED,"COMMENT BLOCK","");
<SEG_COMMENT>"*)"		BEGIN(INITIAL);
<SEG_COMMENT><<EOF>>	invalid_token("UNTERMINATED COMMENT"),BEGIN(INITIAL);
<SEG_COMMENT>.			/* skip content in comment */
<INITIAL,SEG_COMMENT>\n	YIELD commit_token(EOL); // Just test
.						YIELD sprintf(error_info_buffer,"\\x%02x",*yytext),invalid_token("UNIDENTIFIED CHARACTER",error_info_buffer);
%%
