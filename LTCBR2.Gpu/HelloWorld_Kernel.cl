typedef struct Participant
{
    int id;
	int type;
	int purpose;
} Participant;

__kernel void helloworld(__global double *in, __global Participant *p, __global int *out)
{
	int num = get_global_id(0);
	out[num] = p[num].id;
}
