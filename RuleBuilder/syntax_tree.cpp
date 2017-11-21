#include "syntax_tree.h"
#include <stdio.h>
#define MAX_SYNTEX_NODES 65536

node_t *syntax_tree = new node_t[MAX_SYNTEX_NODES];
node_t &syntax_root = syntax_tree[0];
int syntax_tree_size = 1;

int create_node(int type_macro, const char *type_name, int offset_line, int offset_col, int length, int child_count)
{
	int n = syntax_tree_size++;
	syntax_tree[n].type_macro = type_macro;
	syntax_tree[n].type_name = type_name;
	syntax_tree[n].offset_line = offset_line;
	syntax_tree[n].offset_col = offset_col;
	syntax_tree[n].length = length;
	syntax_tree[n].child_count = child_count;
	syntax_tree[n].assigned_child_count = 0;
	syntax_tree[n].child_ids = new int[child_count];
	syntax_tree[n].link_names = new const char*[child_count];
	if(offset_line==-1)
		printf("[Node #%d](%d:%s)Children: %d\n", n, type_macro, type_name, child_count);
	else
		printf("[Node #%d](%d:%s)Pos: <%d,%d,%d> Children: %d\n", n, type_macro, type_name, offset_line, offset_col, length, child_count);
	return n;
}

int create_link(int father_id, int child_id, const char *link_name)
{
	int nc = syntax_tree[father_id].assigned_child_count++;
	syntax_tree[father_id].child_ids[nc] = child_id;
	syntax_tree[father_id].link_names[nc] = link_name;
	printf("[Link #%d -> #%d]%s\n", father_id, child_id, link_name);
	return father_id;
}