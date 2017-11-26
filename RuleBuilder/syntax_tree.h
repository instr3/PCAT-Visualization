#pragma once

struct offset_t
{
	int line, col, pos;
};

struct node_t
{
	int id;
	int type_macro;
	const char *type_name;
	int offset;
	int length;
	int child_count;
	int assigned_child_count;
	node_t** children;
	const char** link_names;
	node_t *append(node_t *child, const char *link_name, bool selective = false);
	node_t *rebound(node_t *child);
	void print(int depth, const char *link_name);
	void output();
};

node_t *create_node(int type_macro, const char *type_name, int offset, int length, int child_count);

void visualize_syntax_tree();
void output_syntax_tree();

#define create_virtual_node(type_name, child_count) (create_node(0,type_name,-1,-1,child_count))

// Create helpers
#define helper_quinop(type,sid1,sid2,sid3,sid4,sid5) (create_virtual_node(#type,5)->append((sid1), "1_op", true)->append((sid2), "2_op", true)->append((sid3), "3_op", true)->append((sid4), "4_op", true)->append((sid5), "5_op", true))

#define helper_quadop(type,sid1,sid2,sid3,sid4) (create_virtual_node(#type,4)->append((sid1), "1_op", true)->append((sid2), "2_op", true)->append((sid3), "3_op", true)->append((sid4), "4_op", true))

#define helper_triop(type,sid1,sid2,sid3) (create_virtual_node(#type,3)->append((sid1), "1_op", true)->append((sid2), "2_op", true)->append((sid3), "3_op", true))

#define helper_biop(type,sid1,sid2) (create_virtual_node(#type,2)->append((sid1), "l_op", true)->append((sid2), "r_op", true))

#define helper_uniop(type,sid) (create_virtual_node(#type,1)->append((sid), "op", true))