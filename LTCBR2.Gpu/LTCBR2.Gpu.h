// LTCBR2.Gpu.h

#pragma once

#include <vector>
#include <iostream>

#include "CL\opencl.h"



namespace LTCBR2Gpu {

	static const char* getOpenCLErrorCodeStr(std::string input)
	{
		return "unknown error code";
	}

	template<typename T>
	static const char* getOpenCLErrorCodeStr(T input)
	{
		int errorCode = (int)input;
		switch (errorCode)
		{
		case CL_DEVICE_NOT_FOUND:
			return "CL_DEVICE_NOT_FOUND";
		case CL_DEVICE_NOT_AVAILABLE:
			return "CL_DEVICE_NOT_AVAILABLE";
		case CL_COMPILER_NOT_AVAILABLE:
			return "CL_COMPILER_NOT_AVAILABLE";
		case CL_MEM_OBJECT_ALLOCATION_FAILURE:
			return "CL_MEM_OBJECT_ALLOCATION_FAILURE";
		case CL_OUT_OF_RESOURCES:
			return "CL_OUT_OF_RESOURCES";
		case CL_OUT_OF_HOST_MEMORY:
			return "CL_OUT_OF_HOST_MEMORY";
		case CL_PROFILING_INFO_NOT_AVAILABLE:
			return "CL_PROFILING_INFO_NOT_AVAILABLE";
		case CL_MEM_COPY_OVERLAP:
			return "CL_MEM_COPY_OVERLAP";
		case CL_IMAGE_FORMAT_MISMATCH:
			return "CL_IMAGE_FORMAT_MISMATCH";
		case CL_IMAGE_FORMAT_NOT_SUPPORTED:
			return "CL_IMAGE_FORMAT_NOT_SUPPORTED";
		case CL_BUILD_PROGRAM_FAILURE:
			return "CL_BUILD_PROGRAM_FAILURE";
		case CL_MAP_FAILURE:
			return "CL_MAP_FAILURE";
		case CL_MISALIGNED_SUB_BUFFER_OFFSET:
			return "CL_MISALIGNED_SUB_BUFFER_OFFSET";
		case CL_EXEC_STATUS_ERROR_FOR_EVENTS_IN_WAIT_LIST:
			return "CL_EXEC_STATUS_ERROR_FOR_EVENTS_IN_WAIT_LIST";
		case CL_INVALID_VALUE:
			return "CL_INVALID_VALUE";
		case CL_INVALID_DEVICE_TYPE:
			return "CL_INVALID_DEVICE_TYPE";
		case CL_INVALID_PLATFORM:
			return "CL_INVALID_PLATFORM";
		case CL_INVALID_DEVICE:
			return "CL_INVALID_DEVICE";
		case CL_INVALID_CONTEXT:
			return "CL_INVALID_CONTEXT";
		case CL_INVALID_QUEUE_PROPERTIES:
			return "CL_INVALID_QUEUE_PROPERTIES";
		case CL_INVALID_COMMAND_QUEUE:
			return "CL_INVALID_COMMAND_QUEUE";
		case CL_INVALID_HOST_PTR:
			return "CL_INVALID_HOST_PTR";
		case CL_INVALID_MEM_OBJECT:
			return "CL_INVALID_MEM_OBJECT";
		case CL_INVALID_IMAGE_FORMAT_DESCRIPTOR:
			return "CL_INVALID_IMAGE_FORMAT_DESCRIPTOR";
		case CL_INVALID_IMAGE_SIZE:
			return "CL_INVALID_IMAGE_SIZE";
		case CL_INVALID_SAMPLER:
			return "CL_INVALID_SAMPLER";
		case CL_INVALID_BINARY:
			return "CL_INVALID_BINARY";
		case CL_INVALID_BUILD_OPTIONS:
			return "CL_INVALID_BUILD_OPTIONS";
		case CL_INVALID_PROGRAM:
			return "CL_INVALID_PROGRAM";
		case CL_INVALID_PROGRAM_EXECUTABLE:
			return "CL_INVALID_PROGRAM_EXECUTABLE";
		case CL_INVALID_KERNEL_NAME:
			return "CL_INVALID_KERNEL_NAME";
		case CL_INVALID_KERNEL_DEFINITION:
			return "CL_INVALID_KERNEL_DEFINITION";
		case CL_INVALID_KERNEL:
			return "CL_INVALID_KERNEL";
		case CL_INVALID_ARG_INDEX:
			return "CL_INVALID_ARG_INDEX";
		case CL_INVALID_ARG_VALUE:
			return "CL_INVALID_ARG_VALUE";
		case CL_INVALID_ARG_SIZE:
			return "CL_INVALID_ARG_SIZE";
		case CL_INVALID_KERNEL_ARGS:
			return "CL_INVALID_KERNEL_ARGS";
		case CL_INVALID_WORK_DIMENSION:
			return "CL_INVALID_WORK_DIMENSION";
		case CL_INVALID_WORK_GROUP_SIZE:
			return "CL_INVALID_WORK_GROUP_SIZE";
		case CL_INVALID_WORK_ITEM_SIZE:
			return "CL_INVALID_WORK_ITEM_SIZE";
		case CL_INVALID_GLOBAL_OFFSET:
			return "CL_INVALID_GLOBAL_OFFSET";
		case CL_INVALID_EVENT_WAIT_LIST:
			return "CL_INVALID_EVENT_WAIT_LIST";
		case CL_INVALID_EVENT:
			return "CL_INVALID_EVENT";
		case CL_INVALID_OPERATION:
			return "CL_INVALID_OPERATION";
		case CL_INVALID_GL_OBJECT:
			return "CL_INVALID_GL_OBJECT";
		case CL_INVALID_BUFFER_SIZE:
			return "CL_INVALID_BUFFER_SIZE";
		case CL_INVALID_MIP_LEVEL:
			return "CL_INVALID_MIP_LEVEL";
		case CL_INVALID_GLOBAL_WORK_SIZE:
			return "CL_INVALID_GLOBAL_WORK_SIZE";
		case CL_INVALID_GL_SHAREGROUP_REFERENCE_KHR:
			return "CL_INVALID_GL_SHAREGROUP_REFERENCE_KHR";
		case CL_PLATFORM_NOT_FOUND_KHR:
			return "CL_PLATFORM_NOT_FOUND_KHR";
			//case CL_INVALID_PROPERTY_EXT:
			//    return "CL_INVALID_PROPERTY_EXT";
		case CL_DEVICE_PARTITION_FAILED_EXT:
			return "CL_DEVICE_PARTITION_FAILED_EXT";
		case CL_INVALID_PARTITION_COUNT_EXT:
			return "CL_INVALID_PARTITION_COUNT_EXT";
		case CL_INVALID_DEVICE_QUEUE:
			return "CL_INVALID_DEVICE_QUEUE";
		case CL_INVALID_PIPE_SIZE:
			return "CL_INVALID_PIPE_SIZE";

		default:
			return "unknown error code";
		}
	}

#define KERNEL_ITERATIONS 100
#define IS_SUCCESS 0
#define IS_FAILURE 1
#define CHECK_OPENCL_ERROR(actual, msg) \
    if(checkVal(actual, CL_SUCCESS, msg)) \
    { \
        std::cout << "Location : " << __FILE__ << ":" << __LINE__<< std::endl; \
        return IS_FAILURE; \
    }
#define CHECK_ALLOCATION(actual, msg) \
    if(actual == NULL) \
    { \
        error(msg); \
        std::cout << "Location : " << __FILE__ << ":" << __LINE__<< std::endl; \
        return IS_FAILURE; \
    }
	/**
	* error
	* constant function, Prints error messages
	* @param errorMsg std::string message
	*/
	static void error(std::string errorMsg)
	{
		std::cout << "Error: " << std::endl;
	}

	template<typename T>
	static int checkVal(
		T input,
		T reference,
		std::string message, bool isAPIerror = true)
	{
		if (input == reference)
		{
			return IS_SUCCESS;
		}
		else
		{
			if (isAPIerror)
			{
				std::cout << "Error: " << " Error code : ";
				std::cout << getOpenCLErrorCodeStr(input) << std::endl;
			}
			else
			{
				error(message);
			}
			return IS_FAILURE;
		}
	}


	class __declspec(dllexport) Gpu
	{
	public:
		Gpu() {	}

		std::vector<std::string> GetPlatforms();
		std::vector<std::string> GetDevices();
	};



	class __declspec(dllexport) Device
	{
	public:

		//CL Objects and memory buffers
		int status;
		cl_device_type dType;       //device type
		cl_device_id deviceId;      //device ID
		cl_context context;         //context
		cl_command_queue queue;     //command-queue
		cl_mem inputBuffer;         //input buffer
		cl_mem outputBuffer;        //output buffer
		cl_program program;         //program object
		cl_kernel kernel;           //kernel object
		cl_event eventObject;       //event object
		cl_ulong kernelStartTime;   //kernel start time
		cl_ulong kernelEndTime;     //kernel end time
		double elapsedTime;         //elapsed time in ms
		cl_float *output;           //output host buffer for verification

		Device()
		{
			output = 0;
		}


		// Create Context
		// @ return IS_SUCCESS on success and IS_FAILURE on failure
		int createContext();

		// Create Command-queue
		// @ return IS_SUCCESS on success and IS_FAILURE on failure
		int createQueue();

		// Create input and output buffers and Enqueue write data
		// @ return IS_SUCCESS on success and IS_FAILURE on failure
		int createBuffers();

		// Initialize Input buffers
		// @ return IS_SUCCESS on success and IS_FAILURE on failure
		int enqueueWriteBuffer();

		// Create Program object
		// @ return IS_SUCCESS on success and IS_FAILURE on failure
		int createProgram(const char **source, const size_t *sourceSize);

		// Build Program source
		// @ return IS_SUCCESS on success and IS_FAILURE on failure
		int buildProgram();

		// Create Kernel object
		// @ return IS_SUCCESS on success and IS_FAILURE on failure
		int createKernel();

		// Set Kernel arguments
		// @ return IS_SUCCESS on success and IS_FAILURE on failure
		int setKernelArgs();

		// Enqueue NDRAnge kernel
		// @ return IS_SUCCESS on success and IS_FAILURE on failure
		int enqueueKernel(size_t *globalThreads, size_t *localThreads);

		// Wait for kernel execution to finish
		// @ return IS_SUCCESS on success and IS_FAILURE on failure
		int waitForKernel();

		// Get kernel execution time
		// @ return IS_SUCCESS on success and IS_FAILURE on failure
		int getProfilingData();

		// Get output data from device
		// @ return IS_SUCCESS on success and IS_FAILURE on failure
		int enqueueReadData();

		// Verify results against host computation
		// @ return IS_SUCCESS on success and IS_FAILURE on failure
		int verifyResults();

		// Cleanup allocated resources
		// @ return IS_SUCCESS on success and IS_FAILURE on failure
		int cleanupResources();

		int myFunction();

		int displayPlatformAndDevices(cl_platform_id platform,
			const cl_device_id* device, const int deviceCount);

	};
	// Context properties
	const cl_context_properties* cprops;
	cl_context_properties cps[3];
	cl_platform_id platform = NULL;
	
	// Size of input data
	int width;

	// Input data is same for all devices
	cl_float *input;

	// Host Output data for verification
	cl_float *verificationOutput;

	// Count for verification
	cl_uint verificationCount = 0;
	cl_uint requiredCount = 0;
	
#define LltcbrGpuApi __declspec(dllexport)


	
}
