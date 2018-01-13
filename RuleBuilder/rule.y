%{
#include <iostream>
#include <vector>
using namespace std;

#include "rule.flex.cpp"
#include "../syntax_tree.h"
void yyerror(const char* msg) {
	extern struct offset_t token_offset;
	printf("Compile error near line %d, col %d\n",token_offset.line, token_offset.col);
	puts(msg);
	if(token_offset.pos<0x10000)
		exit(-(token_offset.pos+0x10000)); // Return code containing error position
	else
		exit(-0x1);
}

node_t *extract_id_list(const char *type_name, vector<node_t *> *s1, node_t *s2=NULL) 
{
	bool extra=s2!=NULL; // Whether an extra '(' or '{' or '[<' is included to calculate bounds
	/*if(!extra && s1->size()==1) // Single node without brackets
	{
		s2=s1->front();
		delete s1; // Release memory! 
		return s2;
	}
	else // Create new node
	*/
	{
		node_t *ss = create_virtual_node(type_name,extra?s1->size()-1:s1->size());
		for(vector<node_t *>::iterator p = extra ? ++s1->begin() : s1->begin(); p != s1->end(); ++p) 
			ss->append(*p,"element"); 
		ss->rebound(s1->front());
		if(extra)
			ss->rebound(s2); 
		delete s1; // Release memory! 
		return ss;
	}
}

%}

%union {
    struct node_t* id;
    vector<node_t *> *ids;
}

%error-verbose
%token <id> NUMBER
%token <id> INVALID ID STRING IGNORED
%token <id> PROGRAM IS BEGINT END VAR TYPE PROCEDURE ARRAY RECORD
       IN OUT READ WRITE IF THEN ELSE ELSIF WHILE DO LOOP
       FOR EXIT RETURN TO BY AND OR NOT OF DIV MOD
       LPAREN  RPAREN LBRACKET RBRACKET LBRACE RBRACE COLON DOT
       SEMICOLON COMMA ASSIGN PLUS MINUS STAR SLASH BACKSLASH EQ
       NEQ LT LE GT GE LABRACKET RABRACKET TRUE FALSE NIL
%token <id> EOL EOFT
/*%type <id> exp
%type <id> factor
%type <id> term*/
%type <id> expression lvalue type program body 
%type <id> or_operand and_operand relationship summand factor unary term
%type <ids> actual_params_suffix comp_values_suffix array_values_suffix write_params_suffix
	declaration_list_suffix var_decl_list_suffix type_decl_list_suffix procedure_decl_list_suffix component_list_suffix
	formal_params_suffix id_list_suffix statement_list_suffix lvalue_list_suffix elsif_sentence_list_suffix
%type <id> actual_params comp_value comp_values array_value array_values write_expr write_params
	declaration_list declaration var_decl_list var_decl type_decl_list type_decl procedure_decl_list procedure_decl
	component_list component formal_params fp_section id_list statement_list statement lvalue_list
	elsif_sentence_list elsif_sentence

%%
calc:
	| calc statement EOL { cout << "Statement, root = " << $2->id << endl; }
	| calc expression EOL { cout << "Expression, root = " << $2->id << endl; }
	| calc program EOFT { extern node_t *syntax_root; syntax_root = $2; return 0;}
	;

program: PROGRAM IS body SEMICOLON { $$ = helper_uniop(program#program_body,$3)->rebound($1)->rebound($4); }

body: declaration_list BEGINT statement_list END {$$=helper_biop(body#declarations#process,$1,$3)->rebound($4);}
	| BEGINT statement_list END {$$=helper_uniop(body#process,$2)->rebound($1)->rebound($3);}

declaration_list: declaration_list_suffix {$$=extract_id_list("declaration_list",$1);}

declaration_list_suffix: declaration {$$=new vector<node_t *>();$$->push_back($1);}
	| declaration_list_suffix declaration {$$=$1;$$->push_back($2);}

declaration: VAR var_decl_list {$$ = $2->rebound($1);}
	| TYPE type_decl_list {$$ = $2->rebound($1);}
	| PROCEDURE procedure_decl_list {$$ = $2->rebound($1);}

var_decl_list: var_decl_list_suffix {$$=extract_id_list("var_decl_list",$1);}

var_decl_list_suffix: var_decl {$$=new vector<node_t *>();$$->push_back($1);}
	| var_decl_list_suffix var_decl {$$=$1;$$->push_back($2);}

var_decl: id_list ASSIGN expression SEMICOLON {$$ = helper_biop(var_decl#var_names#init_value,$1,$3)->rebound($4);}
	| id_list COLON type ASSIGN expression SEMICOLON {$$ = helper_triop(var_decl#var_names#var_type#init_value,$1,$3,$5)->rebound($6);}

type_decl_list: type_decl_list_suffix {$$=extract_id_list("type_decl_list",$1);}

type_decl_list_suffix: type_decl {$$=new vector<node_t *>();$$->push_back($1);}
	| type_decl_list_suffix type_decl {$$=$1;$$->push_back($2);}

type_decl: ID IS type SEMICOLON {$$ = helper_biop(type_decl#id#type_name,$1,$3)->rebound($4);}

procedure_decl_list: procedure_decl_list_suffix {$$=extract_id_list("procedure_decl_list",$1);}

procedure_decl_list_suffix: procedure_decl {$$=new vector<node_t *>();$$->push_back($1);}
	| procedure_decl_list_suffix procedure_decl {$$=$1;$$->push_back($2);}

procedure_decl: ID formal_params IS body SEMICOLON {$$ = helper_triop(procedure_decl#procedure_name#parameter_list#procedure_body,$1,$2,$4)->rebound($5);}
	| ID formal_params COLON type IS body SEMICOLON {$$ = helper_quadop(procedure_decl#function_name#parameter_list#return_type#procedure_body,$1,$2,$4,$6)->rebound($7);}

type: ID
	| ARRAY OF type {$$ = helper_uniop(array_type#array_type,$3)->rebound($1);}
	| RECORD component_list END {$$ = helper_uniop(record_components#recorded_components,$2)->rebound($1)->rebound($3);}

component_list: component_list_suffix {$$=extract_id_list("component_list",$1);}

component_list_suffix: component {$$=new vector<node_t *>();$$->push_back($1);}
	| component_list_suffix component {$$=$1;$$->push_back($2);}

component: ID COLON type SEMICOLON {$$ = helper_biop(component#component_id#component_type,$1,$3)->rebound($4);}

formal_params: LPAREN RPAREN {$$ = create_virtual_node("formal_params",0)->rebound($1)->rebound($2);} // 0 fp_section situation
	| formal_params_suffix RPAREN {$$=extract_id_list("formal_params",$1,$2);}

formal_params_suffix: LPAREN fp_section {$$=new vector<node_t *>();$$->push_back($1);$$->push_back($2);} // An extra '(' is included to calculate bounds
	| formal_params_suffix SEMICOLON fp_section {$$=$1;$$->push_back($3);}

fp_section: id_list COLON type {$$ = helper_biop(fp_section#parameter_name#parameter_type,$1,$3);}

id_list: id_list_suffix {$$=extract_id_list("id_list",$1);}

id_list_suffix: ID {$$=new vector<node_t *>();$$->push_back($1);}
	| id_list_suffix COMMA ID {$$=$1;$$->push_back($3);}

lvalue_list: lvalue_list_suffix {$$=extract_id_list("lvalue_list",$1);}

lvalue_list_suffix: ID {$$=new vector<node_t *>();$$->push_back($1);}
	| lvalue_list_suffix COMMA ID {$$=$1;$$->push_back($3);}

statement_list: statement_list_suffix {$$=extract_id_list("statement_list",$1);}

statement_list_suffix: statement {$$=new vector<node_t *>();$$->push_back($1);}
	| statement_list_suffix statement {$$=$1;$$->push_back($2);}

elsif_sentence_list: elsif_sentence_list_suffix {$$=extract_id_list("elsif_sentence_list",$1);} 

elsif_sentence_list_suffix: elsif_sentence {$$=new vector<node_t *>();$$->push_back($1);}
	| elsif_sentence_list_suffix elsif_sentence {$$=$1;$$->push_back($2);}

elsif_sentence: ELSIF expression THEN statement_list { $$ = helper_biop(elsif_sentence#elsif_condition#elsif_true_part,$2,$4)->rebound($1); }

statement: lvalue ASSIGN expression SEMICOLON { $$ = helper_biop(assign#l_value#r_value,$1,$3)->rebound($4); }
	| ID actual_params SEMICOLON { $$ = helper_biop(procedure_call#procedure_name#parameter_list,$1,$2)->rebound($3); }
	| READ LPAREN lvalue_list RPAREN SEMICOLON { $$ = helper_uniop(read#things_to_read,$3)->rebound($1)->rebound($5); }
	| WRITE write_params SEMICOLON { $$ = helper_uniop(write#things_to_write,$2)->rebound($1)->rebound($3); }
	| IF expression THEN statement_list END SEMICOLON { $$ = helper_biop(if#if_condition#if_true_part,$2,$4)->rebound($1)->rebound($6); }
	| IF expression THEN statement_list elsif_sentence_list END SEMICOLON { $$ = helper_triop(if#if_condition#if_true_part#elsif_part,$2,$4,$5)->rebound($1)->rebound($7); }
	| IF expression THEN statement_list ELSE statement_list END SEMICOLON { $$ = helper_triop(if_else#if_condition#if_true_part#else_part,$2,$4,$6)->rebound($1)->rebound($8); }
	| IF expression THEN statement_list elsif_sentence_list ELSE statement_list END SEMICOLON { $$ = helper_quadop(if_else#if_condition#if_true_part#elsif_part#else_part,$2,$4,$5,$7)->rebound($1)->rebound($9); }
	| WHILE expression DO statement_list END SEMICOLON { $$ = helper_biop(while#while_condition#loop_statements,$2,$4)->rebound($1)->rebound($6); }
	| LOOP statement_list END SEMICOLON { $$ = helper_uniop(loop#loop_statements,$2)->rebound($1)->rebound($4); }
	| FOR ID ASSIGN expression TO expression DO statement_list END SEMICOLON { $$ = helper_quadop(for#loop_variable#loop_init_value#loop_end_value#loop_statements,$2,$4,$6,$8)->rebound($1)->rebound($10); }
	| FOR ID ASSIGN expression TO expression BY expression DO statement_list END SEMICOLON { $$ = helper_quinop(for#loop_variable#loop_init_value#loop_end_value#loop_step_size#loop_statements,$2,$4,$6,$8,$10)->rebound($1)->rebound($12); }
	| EXIT SEMICOLON { $$ = create_virtual_node("exit",0)->rebound($1)->rebound($2); }
	| RETURN expression SEMICOLON { $$ = helper_uniop(return#return_value,$2)->rebound($1)->rebound($3); }
	// Todo: Add more

write_params: LPAREN RPAREN {$$ = create_virtual_node("write_params",0)->rebound($1)->rebound($2);} // 0 parameter situation
	| write_params_suffix RPAREN {$$=extract_id_list("write_params",$1,$2);}

write_params_suffix: LPAREN write_expr {$$=new vector<node_t *>();$$->push_back($1);$$->push_back($2);} // An extra '(' is included to calculate bounds
	| write_params_suffix COMMA write_expr {$$=$1;$$->push_back($3);}

write_expr: STRING
	| expression

expression: or_operand

or_operand: and_operand
	| or_operand OR and_operand { $$ = helper_biop(logical_or,$1,$3); }

and_operand: relationship
	| and_operand AND relationship { $$ = helper_biop(logical_and,$1,$3); }

relationship: summand
	| summand LT summand { $$ = helper_biop(less_than,$1,$3); }
	| summand GT summand { $$ = helper_biop(greater_than,$1,$3); }
	| summand LE summand { $$ = helper_biop(less_equal,$1,$3); }
	| summand GE summand { $$ = helper_biop(greater_equal,$1,$3); }
	| summand EQ summand { $$ = helper_biop(equal,$1,$3); }
	| summand NEQ summand { $$ = helper_biop(not_equal,$1,$3); }

summand: factor
	| summand PLUS factor { $$ = helper_biop(add,$1,$3); } 
	| summand MINUS factor { $$ = helper_biop(subtract,$1,$3); } 

factor: unary
	| factor STAR unary { $$ = helper_biop(multiply,$1,$3); } 
	| factor SLASH unary { $$ = helper_biop(divide,$1,$3); } 
	| factor DIV unary { $$ = helper_biop(div,$1,$3); } 
	| factor MOD unary { $$ = helper_biop(mod,$1,$3); } 

unary: term
	| PLUS unary { $$ = helper_uniop(positive,$2)->rebound($1); }
	| MINUS unary { $$ = helper_uniop(negative,$2)->rebound($1); }
	| NOT unary { $$ = helper_uniop(logical_not,$2)->rebound($1); }

term: NUMBER
	| lvalue
	| LPAREN expression RPAREN { $$ = helper_uniop(brackets,$2)->rebound($1)->rebound($3); }
	| ID actual_params { $$ = helper_biop(function_call,$1,$2); }
	| ID comp_values { $$ = helper_biop(record_construction,$1,$2); }
	| ID array_values { $$ = helper_biop(array_construction,$1,$2); }
	| TRUE { $$ = $1; }
	| FALSE { $$ = $1; }
	| NIL { $$ = $1; }
	;

lvalue: ID
	| lvalue LBRACKET expression RBRACKET { $$ = helper_biop(element_deref#array_name#array_index,$1,$3)->rebound($4); }
	| lvalue DOT ID { $$ = helper_biop(comp_deref#comp_name#comp_index,$1,$3); }
	;
actual_params: LPAREN RPAREN {$$ = create_virtual_node("actual_params",0)->rebound($1)->rebound($2);} // 0 parameter situation
	| actual_params_suffix RPAREN {$$=extract_id_list("actual_params",$1,$2);}

actual_params_suffix: LPAREN expression {$$=new vector<node_t *>();$$->push_back($1);$$->push_back($2);} // An extra '(' is included to calculate bounds
	| actual_params_suffix COMMA expression {$$=$1;$$->push_back($3);}

comp_values: comp_values_suffix RBRACE {$$=extract_id_list("comp_values",$1,$2);}

comp_values_suffix: LBRACE comp_value {$$=new vector<node_t *>();$$->push_back($1);$$->push_back($2);} // An extra '{' is included to calculate bounds
	| comp_values_suffix SEMICOLON comp_value {$$=$1;$$->push_back($3);}

comp_value: ID ASSIGN expression {$$=helper_biop(comp_value_pair#l_value#r_value,$1,$3);}

array_values: array_values_suffix RABRACKET {$$=extract_id_list("array_values",$1,$2);}

array_values_suffix: LABRACKET array_value {$$=new vector<node_t *>();$$->push_back($1);$$->push_back($2);} // An extra '[<' is included to calculate bounds
	| array_values_suffix COMMA array_value {$$=$1;$$->push_back($3);}


array_value: expression
	| expression OF expression {$$=helper_biop(array_value_pair#elements_count#elements_value,$1,$3);}

%%
