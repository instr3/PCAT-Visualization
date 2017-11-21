#pragma once

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
	node_t *append(node_t *child, const char *link_name);
	node_t *rebound(node_t *child);
};

node_t *create_node(int type_macro, const char *type_name, int offset, int length, int child_count);

#define create_virtual_node(type_name, child_count) (create_node(0,type_name,-1,-1,child_count))

// Create helpers
#define helper_quinop(type,sid1,sid2,sid3,sid4,sid5) (create_virtual_node(#type,5)->append((sid1), "1_op")->append((sid2),"2_op")->append((sid3),"3_op")->append((sid4),"4_op")->append((sid5),"5_op"))

#define helper_quadop(type,sid1,sid2,sid3,sid4) (create_virtual_node(#type,4)->append((sid1), "1_op")->append((sid2),"2_op")->append((sid3),"3_op")->append((sid4),"4_op"))

#define helper_triop(type,sid1,sid2,sid3) (create_virtual_node(#type,3)->append((sid1), "1_op")->append((sid2),"2_op")->append((sid3),"3_op"))

#define helper_biop(type,sid1,sid2) (create_virtual_node(#type,2)->append((sid1), "l_op")->append((sid2),"r_op"))

#define helper_uniop(type,sid) (create_virtual_node(#type,1)->append((sid), "op"))