#include "syntax_tree.h"
#include <stdio.h>
#include <algorithm>
#include <string>
#include <assert.h>
using namespace std;
#define MAX_SYNTEX_NODES 65536

node_t *syntax_tree = new node_t[MAX_SYNTEX_NODES];
node_t *syntax_root = NULL;
int syntax_tree_size = 1;
const char *delimiter = "#";

node_t *create_node(int type_macro, const char *type_name, int offset, int length, int child_count)
{
	int n = syntax_tree_size++;
	syntax_tree[n].id = n;
	syntax_tree[n].type_macro = type_macro;
	char *temp = new char[strlen(type_name) + 1];
	strcpy(temp, type_name);
	char *result = strtok(temp, delimiter);
	assert(result);
	syntax_tree[n].type_name = result;
	result = strtok(NULL, delimiter);
	syntax_tree[n].offset = offset;
	syntax_tree[n].length = length;
	syntax_tree[n].child_count = child_count;
	syntax_tree[n].assigned_child_count = 0;
	syntax_tree[n].children = new node_t*[child_count];
	syntax_tree[n].link_names = new const char*[child_count]();
	int tid = 0;
	while (result)
	{
		assert(tid < syntax_tree[n].child_count);
		syntax_tree[n].link_names[tid++] = result;
		result = strtok(NULL, delimiter);
	}
	/* if(length == -1)
		printf("[Node #%d](%d:%s)Children: %d\n", n, type_macro, type_name, child_count);
	else
		printf("[Node #%d](%d:%s)Pos: <%d,%d> Children: %d\n", n, type_macro, type_name, offset, length, child_count);
	*/
	return &syntax_tree[n];
}

void visualize_syntax_tree()
{
	syntax_root->print(0, "root");
}

void output_syntax_tree()
{
	printf("%d\n", syntax_tree_size);
	syntax_root->output();
}

node_t * node_t::append(node_t * child, const char * link_name, bool selective)
{
	int nc = assigned_child_count++;
	assert(nc < child_count);
	children[nc] = child;
	if (selective && link_names[nc])
	{
		link_name = link_names[nc];
	}
	link_names[nc] = link_name;
	// printf("[Link #%d -> #%d]%s\n", id, child->id, link_name);
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

void node_t::print(int depth,const char *link_name)
{
	for (int i = 0; i < depth; ++i)putchar(' ');
	printf("<%s>%s:%d @ <%d,%d>\n", link_name, type_name, id, offset, length);
	for (int i = 0; i < child_count; ++i)
	{
		children[i]->print(depth + 1, link_names[i]);
	}
}

void node_t::output()
{
	printf("%d %d %s %d %d %d ", id, type_macro, type_name, offset, length, child_count);
	for (int i = 0; i < child_count; ++i)
	{
		printf("%d %s ", children[i]->id, link_names[i]);
	}
	printf("\n");
	for (int i = 0; i < child_count; ++i)
	{
		children[i]->output();
	}
}
