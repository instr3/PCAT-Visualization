#include "syntax_tree.h"
#include <stdio.h>
#include <algorithm>
using namespace std;
#define MAX_SYNTEX_NODES 65536

node_t *syntax_tree = new node_t[MAX_SYNTEX_NODES];
node_t &syntax_root = syntax_tree[0];
int syntax_tree_size = 1;

node_t *create_node(int type_macro, const char *type_name, int offset, int length, int child_count)
{
	int n = syntax_tree_size++;
	syntax_tree[n].id = n;
	syntax_tree[n].type_macro = type_macro;
	syntax_tree[n].type_name = type_name;
	syntax_tree[n].offset = offset;
	syntax_tree[n].length = length;
	syntax_tree[n].child_count = child_count;
	syntax_tree[n].assigned_child_count = 0;
	syntax_tree[n].children = new node_t*[child_count];
	syntax_tree[n].link_names = new const char*[child_count];
	if(length == -1)
		printf("[Node #%d](%d:%s)Children: %d\n", n, type_macro, type_name, child_count);
	else
		printf("[Node #%d](%d:%s)Pos: <%d,%d> Children: %d\n", n, type_macro, type_name, offset, length, child_count);
	return &syntax_tree[n];
}

node_t * node_t::append(node_t * child, const char * link_name)
{
	int nc = assigned_child_count++;
	children[nc] = child;
	link_names[nc] = link_name;
	printf("[Link #%d -> #%d]%s\n", id, child->id, link_name);
	rebound(child);
	return this;
}

node_t * node_t::rebound(node_t * child)
{
	if (length == -1)
	{
		length = child->length;
		offset = child->offset;
	}
	else
	{
		int right = length + offset;
		offset = min(offset, child->offset);
		right = max(right, child->offset + child->length);
		length = right - offset;
	}
	return this;
}
