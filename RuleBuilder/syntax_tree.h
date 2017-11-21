#pragma once

struct node_t
{
	int type_macro;
	const char *type_name;
	int offset_line;
	int offset_col;
	int length;
	int child_count;
	int assigned_child_count;
	int* child_ids;
	const char** link_names;
};

int create_node(int type_macro, const char *type_name, int offset_line, int offset_col, int length, int child_count);

int create_link(int father_id, int child_id, const char *link_name);

#define create_innode(type_name, child_count) create_node(0,type_name,-1,-1,-1,child_count)

// Create helpers
#define helper_biop(type,sid1,sid2) create_link(create_link(create_innode(#type, 2),(sid1), "l_op"),(sid2),"r_op")

#define helper_uniop(type,sid) create_link(create_innode(#type, 1),(sid), "op")