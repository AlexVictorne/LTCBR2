// This is the main DLL file.
#pragma once

#include "stdafx.h"
#include "LTCBR2.Gpu.h"

#include <string.h>
#include <stdio.h>
#include <stdlib.h>
#include <string>
#include <fstream>
#include <array>

using namespace std;

namespace LTCBR2Gpu {

	int Device::createContext()
	{
		//Create context using current device's ID
		context = clCreateContext(cprops,
			1,
			&deviceId,
			0,
			0,
			&status);
		CHECK_OPENCL_ERROR(status, "clCreateContext failed.");

		return IS_SUCCESS;
	}

	int Device::createQueue()
	{
		//Create Command-Queue
		queue = clCreateCommandQueue(context,
			deviceId,
			CL_QUEUE_PROFILING_ENABLE,
			&status);
		CHECK_OPENCL_ERROR(status, "clCreateCommandQueue failed.");

		return IS_SUCCESS;
	}

	int Device::createBuffers()
	{
		// Create input buffer
		inputBuffer = clCreateBuffer(context,
			CL_MEM_READ_ONLY,
			width * sizeof(cl_float),
			0,
			&status);
		CHECK_OPENCL_ERROR(status, "clCreateBuffer failed.(inputBuffer)");

		// Create output buffer
		outputBuffer = clCreateBuffer(context,
			CL_MEM_WRITE_ONLY,
			width * sizeof(cl_float),
			0,
			&status);
		CHECK_OPENCL_ERROR(status, "clCreateBuffer failed.(outputBuffer)");

		return IS_SUCCESS;
	}

	int Device::enqueueWriteBuffer()
	{
		// Initialize input buffer
		status = clEnqueueWriteBuffer(queue,
			inputBuffer,
			1,
			0,
			width * sizeof(cl_float),
			input,
			0,
			0,
			0);
		CHECK_OPENCL_ERROR(status, "clEnqueueWriteBuffer failed.");

		return IS_SUCCESS;
	}

	int Device::createProgram(const char **source, const size_t *sourceSize)
	{
		// Create program with source
		program = clCreateProgramWithSource(context,
			1,
			source,
			sourceSize,
			&status);
		CHECK_OPENCL_ERROR(status, "clCreateProgramWithSource failed.");

		return IS_SUCCESS;
	}

	int Device::buildProgram()
	{
		char buildOptions[50];
		sprintf(buildOptions, "-D KERNEL_ITERATIONS=%d", KERNEL_ITERATIONS);
		// Build program source
		status = clBuildProgram(program,
			1,
			&deviceId,
			buildOptions,
			0,
			0);
		// Print build log here if build program failed
		if (status != CL_SUCCESS)
		{
			if (status == CL_BUILD_PROGRAM_FAILURE)
			{
				cl_int logStatus;
				char *buildLog = NULL;
				size_t buildLogSize = 0;
				logStatus = clGetProgramBuildInfo(program,
					deviceId,
					CL_PROGRAM_BUILD_LOG,
					buildLogSize,
					buildLog,
					&buildLogSize);
				CHECK_OPENCL_ERROR(status, "clGetProgramBuildInfo failed.");

				buildLog = (char*)malloc(buildLogSize);
				if (buildLog == NULL)
				{
					CHECK_ALLOCATION(buildLog, "Failed to allocate host memory. (buildLog)");
				}

				memset(buildLog, 0, buildLogSize);

				logStatus = clGetProgramBuildInfo(program,
					deviceId,
					CL_PROGRAM_BUILD_LOG,
					buildLogSize,
					buildLog,
					NULL);
				if (logStatus != CL_SUCCESS)
				{
					std::cout << "clGetProgramBuildInfo failed.";
					free(buildLog);
					return IS_FAILURE;
				}

				std::cout << " \n\t\t\tBUILD LOG\n";
				std::cout << " ************************************************\n";
				std::cout << buildLog << std::endl;
				std::cout << " ************************************************\n";
				free(buildLog);
			}

			CHECK_OPENCL_ERROR(status, "clBuildProgram failed.");
		}

		return IS_SUCCESS;
	}

	int Device::createKernel()
	{
		kernel = clCreateKernel(program, "multiDeviceKernel", &status);
		CHECK_OPENCL_ERROR(status, "clCreateKernel failed.");

		return IS_SUCCESS;
	}

	int Device::setKernelArgs()
	{
		status = clSetKernelArg(kernel, 0, sizeof(cl_mem), &inputBuffer);
		CHECK_OPENCL_ERROR(status, "clSetKernelArg failed.(inputBuffer)");

		status = clSetKernelArg(kernel, 1, sizeof(cl_mem), &outputBuffer);
		CHECK_OPENCL_ERROR(status, "clSetKernelArg failed.(outputBuffer)");

		return IS_SUCCESS;
	}

	int Device::enqueueKernel(size_t *globalThreads, size_t *localThreads)
	{
		status = clEnqueueNDRangeKernel(queue,
			kernel,
			1,
			NULL,
			globalThreads,
			localThreads,
			0,
			NULL,
			&eventObject);
		CHECK_OPENCL_ERROR(status, "clEnqueueNDRangeKernel failed.");

		status = clFlush(queue);
		CHECK_OPENCL_ERROR(status, "clFlush failed.");

		return IS_SUCCESS;
	}

	int Device::waitForKernel()
	{
		status = clFinish(queue);
		CHECK_OPENCL_ERROR(status, "clFinish failed.");

		return IS_SUCCESS;
	}

	int Device::getProfilingData()
	{
		status = clGetEventProfilingInfo(eventObject,
			CL_PROFILING_COMMAND_START,
			sizeof(cl_ulong),
			&kernelStartTime,
			0);
		CHECK_OPENCL_ERROR(status, "clGetEventProfilingInfo failed.(start time)");

		status = clGetEventProfilingInfo(eventObject,
			CL_PROFILING_COMMAND_END,
			sizeof(cl_ulong),
			&kernelEndTime,
			0);
		CHECK_OPENCL_ERROR(status, "clGetEventProfilingInfo failed.(end time)");

		//Measure time in ms
		elapsedTime = 1e-6 * (kernelEndTime - kernelStartTime);

		return IS_SUCCESS;
	}

	int Device::enqueueReadData()
	{
		// Allocate memory
		if (output == NULL)
		{
			output = (cl_float*)malloc(width * sizeof(cl_float));
			CHECK_ALLOCATION(output, "Failed to allocate output buffer!\n");
		}

		status = clEnqueueReadBuffer(queue,
			outputBuffer,
			1,
			0,
			width * sizeof(cl_float),
			output,
			0, 0, 0);
		CHECK_OPENCL_ERROR(status, "clEnqueueReadBuffer failed.");

		return IS_SUCCESS;
	}

	int Device::verifyResults()
	{
		float error = 0;
		//compare results between verificationOutput and output host buffers
		for (int i = 0; i < width; i++)
		{
			error += (output[i] - verificationOutput[i]);
		}
		error /= width;

		if (error < 0.001)
		{
			std::cout << "Passed!\n" << std::endl;
			verificationCount++;
		}
		else
		{
			std::cout << "Failed!\n" << std::endl;
			return IS_FAILURE;
		}

		return IS_SUCCESS;
	}

	int Device::cleanupResources()
	{
		int status = clReleaseCommandQueue(queue);
		CHECK_OPENCL_ERROR(status, "clReleaseCommandQueue failed.(queue)");

		status = clReleaseKernel(kernel);
		CHECK_OPENCL_ERROR(status, "clReleaseKernel failed.(kernel)");

		cl_uint programRefCount;
		status = clGetProgramInfo(program,
			CL_PROGRAM_REFERENCE_COUNT,
			sizeof(cl_uint),
			&programRefCount,
			0);
		CHECK_OPENCL_ERROR(status, "clGetProgramInfo failed.");

		if (programRefCount)
		{
			status = clReleaseProgram(program);
			CHECK_OPENCL_ERROR(status, "clReleaseProgram failed.");
		}

		cl_uint inputRefCount;
		status = clGetMemObjectInfo(inputBuffer,
			CL_MEM_REFERENCE_COUNT,
			sizeof(cl_uint),
			&inputRefCount,
			0);
		CHECK_OPENCL_ERROR(status, "clGetMemObjectInfo failed.");

		if (inputRefCount)
		{
			status = clReleaseMemObject(inputBuffer);
			CHECK_OPENCL_ERROR(status, "clReleaseMemObject failed. (inputBuffer)");
		}

		cl_uint outputRefCount;
		status = clGetMemObjectInfo(outputBuffer,
			CL_MEM_REFERENCE_COUNT,
			sizeof(cl_uint),
			&outputRefCount,
			0);
		CHECK_OPENCL_ERROR(status, "clGetMemObjectInfo failed.");

		if (outputRefCount)
		{
			status = clReleaseMemObject(outputBuffer);
			CHECK_OPENCL_ERROR(status, "clReleaseMemObject failed. (outputBuffer)");
		}

		cl_uint contextRefCount;
		status = clGetContextInfo(context,
			CL_CONTEXT_REFERENCE_COUNT,
			sizeof(cl_uint),
			&contextRefCount,
			0);
		CHECK_OPENCL_ERROR(status, "clGetContextInfo failed.");

		if (contextRefCount)
		{
			status = clReleaseContext(context);
			CHECK_OPENCL_ERROR(status, "clReleaseContext failed.");
		}

		status = clReleaseEvent(eventObject);
		CHECK_OPENCL_ERROR(status, "clReleaseEvent failed.");

		return IS_SUCCESS;
	}


	int convertToString(const char *filename, std::string& s)
	{
		size_t size;
		char*  str;
		std::fstream f(filename, (std::fstream::in | std::fstream::binary));

		if (f.is_open())
		{
			size_t fileSize;
			f.seekg(0, std::fstream::end);
			size = fileSize = (size_t)f.tellg();
			f.seekg(0, std::fstream::beg);
			str = new char[size + 1];
			if (!str)
			{
				f.close();
				return 0;
			}

			f.read(str, fileSize);
			f.close();
			str[size] = '\0';
			s = str;
			delete[] str;
			return 0;
		}

		return IS_FAILURE;
	}

	int Device::myFunction() {
		std::cout << "Error: " << std::endl;
		/*Step1: Getting platforms and choose an available one.*/
		cl_uint numPlatforms;	//the NO. of platforms
		cl_platform_id platform = NULL;	//the chosen platform
		cl_int	status = clGetPlatformIDs(0, NULL, &numPlatforms);
		if (status != CL_SUCCESS)
		{
			return IS_FAILURE;
		}

		/*For clarity, choose the first available platform. */
		if (numPlatforms > 0)
		{
			cl_platform_id* platforms = (cl_platform_id*)malloc(numPlatforms * sizeof(cl_platform_id));
			status = clGetPlatformIDs(numPlatforms, platforms, NULL);
			platform = platforms[0];
			free(platforms);
		}

		/*Step 2:Query the platform and choose the first GPU device if has one.Otherwise use the CPU as device.*/
		cl_uint				numDevices = 0;
		cl_device_id        *devices;
		status = clGetDeviceIDs(platform, CL_DEVICE_TYPE_GPU, 0, NULL, &numDevices);
		if (numDevices == 0)	//no GPU available.
		{
			status = clGetDeviceIDs(platform, CL_DEVICE_TYPE_CPU, 0, NULL, &numDevices);
			devices = (cl_device_id*)malloc(numDevices * sizeof(cl_device_id));
			status = clGetDeviceIDs(platform, CL_DEVICE_TYPE_CPU, numDevices, devices, NULL);
		}
		else
		{
			devices = (cl_device_id*)malloc(numDevices * sizeof(cl_device_id));
			status = clGetDeviceIDs(platform, CL_DEVICE_TYPE_GPU, numDevices, devices, NULL);
		}


		/*Step 3: Create context.*/
		cl_context context = clCreateContext(NULL, 1, devices, NULL, NULL, NULL);

		/*Step 4: Creating command queue associate with the context.*/
		cl_command_queue commandQueue = clCreateCommandQueue(context, devices[0], 0, NULL);

		/*Step 5: Create program object */
		const char *filename = "HelloWorld_Kernel.cl";
		string sourceStr;
		status = convertToString(filename, sourceStr);
		const char *source = sourceStr.c_str();
		size_t sourceSize[] = { strlen(source) };
		cl_program program = clCreateProgramWithSource(context, 1, &source, sourceSize, NULL);

		/*Step 6: Build program. */
		status = clBuildProgram(program, 1, devices, NULL, NULL, NULL);

		/*Step 7: Initial input,output for the host and create memory objects for the kernel*/
		const char* input = "GdkknVnqkc";
		size_t strlength = strlen(input);
		char *output = (char*)malloc(strlength + 1);

		cl_mem inputBuffer = clCreateBuffer(context, CL_MEM_READ_ONLY | CL_MEM_COPY_HOST_PTR, (strlength + 1) * sizeof(char), (void *)input, NULL);
		cl_mem outputBuffer = clCreateBuffer(context, CL_MEM_WRITE_ONLY, (strlength + 1) * sizeof(char), NULL, NULL);

		/*Step 8: Create kernel object */
		cl_kernel kernel = clCreateKernel(program, "helloworld", NULL);

		/*Step 9: Sets Kernel arguments.*/
		status = clSetKernelArg(kernel, 0, sizeof(cl_mem), (void *)&inputBuffer);
		status = clSetKernelArg(kernel, 1, sizeof(cl_mem), (void *)&outputBuffer);

		/*Step 10: Running the kernel.*/
		size_t global_work_size[1] = { strlength };
		status = clEnqueueNDRangeKernel(commandQueue, kernel, 1, NULL, global_work_size, NULL, 0, NULL, NULL);

		/*Step 11: Read the cout put back to host memory.*/
		status = clEnqueueReadBuffer(commandQueue, outputBuffer, CL_TRUE, 0, strlength * sizeof(char), output, 0, NULL, NULL);

		output[strlength] = '\0';	//Add the terminal character to the end of output.

		/*Step 12: Clean the resources.*/
		status = clReleaseKernel(kernel);				//Release kernel.
		status = clReleaseProgram(program);				//Release the program object.
		status = clReleaseMemObject(inputBuffer);		//Release mem object.
		status = clReleaseMemObject(outputBuffer);
		status = clReleaseCommandQueue(commandQueue);	//Release  Command queue.
		status = clReleaseContext(context);				//Release context.

		if (output != NULL)
		{
			free(output);
			output = NULL;
		}

		if (devices != NULL)
		{
			free(devices);
			devices = NULL;
		}

		std::cout << "Passed!\n";
		return IS_SUCCESS;
	}

	int Device::displayPlatformAndDevices(cl_platform_id platform,
		const cl_device_id* device, const int deviceCount)
	{
		cl_int status;
		// Get platform name
		char platformVendor[1024];
		status = clGetPlatformInfo(platform, CL_PLATFORM_VENDOR, sizeof(platformVendor),
			platformVendor, NULL);
		CHECK_OPENCL_ERROR(status, "clGetPlatformInfo failed");
		std::cout << "\nSelected Platform Vendor : " << platformVendor << std::endl;
		// Print device index and device names
		for (cl_int i = 0; i < deviceCount; ++i)
		{
			char deviceName[1024];
			status = clGetDeviceInfo(device[i], CL_DEVICE_NAME, sizeof(deviceName),
				deviceName, NULL);
			CHECK_OPENCL_ERROR(status, "clGetDeviceInfo failed");
			std::cout << "Device " << i << " : " << deviceName << std::endl;
		}
		return IS_SUCCESS;
	}

	
	struct platform
	{
		string Name;
		string Vendor;
		string Version;
		string Profile;
		string Extensions;
	};

	vector<string> Gpu::GetPlatforms()
	{
		int i, j;
		char* info;
		size_t infoSize;
		cl_uint platformCount;
		cl_platform_id *platforms;
		const char* attributeNames[5] = { "Name", "Vendor", "Version", "Profile", "Extensions" };
		const cl_platform_info attributeTypes[5] = { CL_PLATFORM_NAME, CL_PLATFORM_VENDOR,
			CL_PLATFORM_VERSION, CL_PLATFORM_PROFILE, CL_PLATFORM_EXTENSIONS };
		const int attributeCount = sizeof(attributeNames) / sizeof(char*);
		// get platform count
		clGetPlatformIDs(5, NULL, &platformCount);
		// get all platforms
		platforms = (cl_platform_id*)malloc(sizeof(cl_platform_id) * platformCount);
		clGetPlatformIDs(platformCount, platforms, NULL);
		vector<string> arr;
		
		// for each platform print all attributes
		for (i = 0; i < platformCount; i++) {
			for (j = 0; j < attributeCount; j++) {

				// get platform attribute value size
				clGetPlatformInfo(platforms[i], attributeTypes[j], 0, NULL, &infoSize);
				info = (char*)malloc(infoSize);

				// get platform attribute value
				clGetPlatformInfo(platforms[i], attributeTypes[j], infoSize, info, NULL);

				arr.push_back(info);
				free(info);
			}
		}
		free(platforms);
		return arr;
	}

	vector<string> Gpu::GetDevices()
	{
		int i, j;
		char* value;
		size_t valueSize;
		cl_uint platformCount;
		cl_platform_id* platforms;
		cl_uint deviceCount;
		cl_device_id* devices;
		cl_uint maxComputeUnits;

		// get all platforms
		clGetPlatformIDs(0, NULL, &platformCount);
		platforms = (cl_platform_id*)malloc(sizeof(cl_platform_id) * platformCount);
		clGetPlatformIDs(platformCount, platforms, NULL);

		vector<string> arr;

		for (i = 0; i < platformCount; i++) {

			// get all devices
			clGetDeviceIDs(platforms[i], CL_DEVICE_TYPE_ALL, 0, NULL, &deviceCount);
			devices = (cl_device_id*)malloc(sizeof(cl_device_id) * deviceCount);
			clGetDeviceIDs(platforms[i], CL_DEVICE_TYPE_ALL, deviceCount, devices, NULL);

			// for each device print critical attributes
			for (j = 0; j < deviceCount; j++) {

				// print device name
				clGetDeviceInfo(devices[j], CL_DEVICE_NAME, 0, NULL, &valueSize);
				value = (char*)malloc(valueSize);
				clGetDeviceInfo(devices[j], CL_DEVICE_NAME, valueSize, value, NULL);
				
				arr.push_back(value);
				free(value);
				
				// print hardware device version
				clGetDeviceInfo(devices[j], CL_DEVICE_VERSION, 0, NULL, &valueSize);
				value = (char*)malloc(valueSize);
				clGetDeviceInfo(devices[j], CL_DEVICE_VERSION, valueSize, value, NULL);
				
				arr.push_back(value);
				free(value);

				// print software driver version
				clGetDeviceInfo(devices[j], CL_DRIVER_VERSION, 0, NULL, &valueSize);
				value = (char*)malloc(valueSize);
				clGetDeviceInfo(devices[j], CL_DRIVER_VERSION, valueSize, value, NULL);
				
				arr.push_back(value);
				free(value);

				// print c version supported by compiler for device
				clGetDeviceInfo(devices[j], CL_DEVICE_OPENCL_C_VERSION, 0, NULL, &valueSize);
				value = (char*)malloc(valueSize);
				clGetDeviceInfo(devices[j], CL_DEVICE_OPENCL_C_VERSION, valueSize, value, NULL);
				
				arr.push_back(value);
				free(value);

				// print parallel compute units
				clGetDeviceInfo(devices[j], CL_DEVICE_MAX_COMPUTE_UNITS,
					sizeof(maxComputeUnits), &maxComputeUnits, NULL);
				arr.push_back(to_string(maxComputeUnits));

			}

			free(devices);

		}

		free(platforms);
		return arr;
	}
	
	using namespace System::Runtime::InteropServices;//нужно для SysStringToChar
													 // char* to System::String^
	System::String^  CharToSysString(char* ch)
	{
		char * chr = ch;
		System::String^ str;
		for (int i = 0; chr[i] != '\0'; i++)
		{
			str += wchar_t(ch[i]);
		}
		return str;
	};

	char * SysStringToChar(System::String^ string)
	{
		return (char*)(void*)Marshal::StringToHGlobalAnsi(string);
	}

	System::String^ FileRead(System::String^ fileName)
	{
		System::String^ result = "";
		try
		{
			System::Console::WriteLine("trying to open file {0}...", fileName);
			System::IO::StreamReader^ din = System::IO::File::OpenText(fileName);

			System::String^ str;
			int count = 0;
			while ((str = din->ReadLine()) != nullptr)
			{
				result += str;
			}
		}
		catch (System::Exception^ e)
		{
			if (dynamic_cast<System::IO::FileNotFoundException^>(e))
				System::Console::WriteLine("file '{0}' not found", fileName);
			else
				System::Console::WriteLine("problem reading file '{0}'", fileName);
		}
		return  result;
	}

	typedef struct Participant
	{
		cl_int id;
		cl_int type;
		cl_int purpose;
	} Participant;
	public ref class GpuWork
	{
	private:
		cl_platform_id selectedPlatform;
		cl_device_id selectedDevice;
	public:
		cli::array<System::String^>^ GetPlatforms(void) {
			int i, j;
			char* info;
			size_t infoSize;
			cl_uint platformCount;
			cl_platform_id *platforms;
			const char* attributeNames[5] = { "Name", "Vendor", "Version", "Profile", "Extensions" };
			const cl_platform_info attributeTypes[5] = { CL_PLATFORM_NAME, CL_PLATFORM_VENDOR,
				CL_PLATFORM_VERSION, CL_PLATFORM_PROFILE, CL_PLATFORM_EXTENSIONS };
			const int attributeCount = sizeof(attributeNames) / sizeof(char*);
			// get platform count
			clGetPlatformIDs(5, NULL, &platformCount);
			// get all platforms
			platforms = (cl_platform_id*)malloc(sizeof(cl_platform_id) * platformCount);
			clGetPlatformIDs(platformCount, platforms, NULL);
			cli::array<System::String^>^ strAr = gcnew cli::array<System::String^>(10);
			
			// for each platform print all attributes
			for (i = 0; i < platformCount; i++) {
				for (j = 0; j < attributeCount; j++) {

					// get platform attribute value size
					clGetPlatformInfo(platforms[i], attributeTypes[j], 0, NULL, &infoSize);
					info = (char*)malloc(infoSize);

					// get platform attribute value
					clGetPlatformInfo(platforms[i], attributeTypes[j], infoSize, info, NULL);
					strAr[(j)+(i*5)] = CharToSysString(info);
					free(info);
				}
			}
			free(platforms);
			return strAr;
		}
		void SetPlatform(int platform)
		{
			cl_uint platformCount;
			cl_platform_id* platforms;
			// get all platforms
			clGetPlatformIDs(0, NULL, &platformCount);
			platforms = (cl_platform_id*)malloc(sizeof(cl_platform_id) * platformCount);
			clGetPlatformIDs(platformCount, platforms, NULL);

			selectedPlatform = platforms[platform];

			free(platforms);
		}
		cli::array<System::String^>^ GetDevices(void)
		{
			int i, j;
			char* value;
			size_t valueSize;
			cl_uint deviceCount;
			cl_device_id* devices;
			cl_uint maxComputeUnits;

			cli::array<System::String^>^ strAr = gcnew cli::array<System::String^>(20);

			clGetDeviceIDs(selectedPlatform, CL_DEVICE_TYPE_ALL, 0, NULL, &deviceCount);
			devices = (cl_device_id*)malloc(sizeof(cl_device_id) * deviceCount);
			clGetDeviceIDs(selectedPlatform, CL_DEVICE_TYPE_ALL, deviceCount, devices, NULL);
			i = 0;
			for (j = 0; j < deviceCount; j++) {
				// print device name
				clGetDeviceInfo(devices[j], CL_DEVICE_NAME, 0, NULL, &valueSize);
				value = (char*)malloc(valueSize);
				clGetDeviceInfo(devices[j], CL_DEVICE_NAME, valueSize, value, NULL);

				strAr[i] = CharToSysString(value);
				i++;
				free(value);

				// print hardware device version
				clGetDeviceInfo(devices[j], CL_DEVICE_VERSION, 0, NULL, &valueSize);
				value = (char*)malloc(valueSize);
				clGetDeviceInfo(devices[j], CL_DEVICE_VERSION, valueSize, value, NULL);

				strAr[i] = CharToSysString(value);
				i++;
				free(value);

				// print software driver version
				clGetDeviceInfo(devices[j], CL_DRIVER_VERSION, 0, NULL, &valueSize);
				value = (char*)malloc(valueSize);
				clGetDeviceInfo(devices[j], CL_DRIVER_VERSION, valueSize, value, NULL);

				strAr[i] = CharToSysString(value);
				i++;
				free(value);

				// print c version supported by compiler for device
				clGetDeviceInfo(devices[j], CL_DEVICE_OPENCL_C_VERSION, 0, NULL, &valueSize);
				value = (char*)malloc(valueSize);
				clGetDeviceInfo(devices[j], CL_DEVICE_OPENCL_C_VERSION, valueSize, value, NULL);

				strAr[i] = CharToSysString(value);
				i++;
				free(value);

				// print parallel compute units
				clGetDeviceInfo(devices[j], CL_DEVICE_MAX_COMPUTE_UNITS, sizeof(maxComputeUnits), &maxComputeUnits, NULL);

				strAr[i] = maxComputeUnits.ToString();
				i++;
			}
			free(devices);
			return strAr;
		}
		void SetDevice(int device)
		{
			cl_uint deviceCount;
			cl_device_id* devices;
			clGetDeviceIDs(selectedPlatform, CL_DEVICE_TYPE_ALL, 0, NULL, &deviceCount);
			devices = (cl_device_id*)malloc(sizeof(cl_device_id) * deviceCount);
			clGetDeviceIDs(selectedPlatform, CL_DEVICE_TYPE_ALL, deviceCount, devices, NULL);

			selectedDevice = devices[device];
			free(devices);
		}

		cli::array<System::Double^>^ Initialization(void)
		{
			cl_device_id devices[] = { selectedDevice };
			// Create context.
			cl_context context = clCreateContext(NULL, 1,  devices, NULL, NULL, NULL);

			// Creating command queue associate with the context.
			cl_command_queue commandQueue = clCreateCommandQueue(context, selectedDevice, 0, NULL);

			// Create program object 
			//const char *filename = "HelloWorld_Kernel.cl";
			//string sourceStr;
			//auto state = convertToString(filename, sourceStr);
			//const char *source = sourceStr.c_str();
			//

			

			const char *source = SysStringToChar(FileRead("HelloWorld_Kernel.cl"));
			size_t sourceSize[] = { strlen(source) };
			cl_program program = clCreateProgramWithSource(context, 1, &source, sourceSize, NULL);

			// Build program. 
			auto state = clBuildProgram(program, 1, devices, NULL, NULL, NULL);

			/*Step 8: Create kernel object */
			cl_kernel kernel = clCreateKernel(program, "helloworld", NULL);

			// Initial input,output for the host and create memory objects for the kernel
			// Length of vectors
			unsigned int n = 1000;

			// Host input vectors
			double *h_a;
			// Host output vector
			double *h_c;

			// Device input buffers
			cl_mem d_a;
			cl_mem d_b;
			// Device output buffer
			cl_mem d_c;

			// Size, in bytes, of each vector
			size_t bytes = n * sizeof(double);

			// Allocate memory for each vector on host
			h_a = (double*)malloc(bytes);
			h_c = (double*)malloc(bytes);

			// Initialize vectors on host
			int i;
			for (i = 0; i < n; i++)
			{
				h_a[i] = i;
			}


			Participant* participants = new Participant[n];
			for (i = 0; i < n; i++)
			{
				participants[i].id = 1;
				participants[i].purpose = 2;
				participants[i].type = 3;
			}
			d_b = clCreateBuffer(context, CL_MEM_READ_ONLY, sizeof(Participant)*n, NULL, NULL);
			
			clEnqueueWriteBuffer(commandQueue, d_b, CL_TRUE, 0, sizeof(Participant)*n, &participants, 0, NULL, NULL);
			
			clSetKernelArg(kernel, 1, sizeof(Participant)*n, &d_b);


			d_a = clCreateBuffer(context, CL_MEM_READ_ONLY, bytes, NULL, NULL);
			d_c = clCreateBuffer(context, CL_MEM_WRITE_ONLY, bytes, NULL, NULL);

			// Write our data set into the input array in device memory
			clEnqueueWriteBuffer(commandQueue, d_a, CL_TRUE, 0, bytes, h_a, 0, NULL, NULL);
			// Set the arguments to our compute kernel
			clSetKernelArg(kernel, 0, sizeof(cl_mem), &d_a);
			clSetKernelArg(kernel, 2, sizeof(cl_mem), &d_c);

			//const char* input = "GdkknVnqkc";
			//size_t strlength = strlen(input);
			//char *output = (char*)malloc(strlength + 1);

			//cl_mem inputBuffer = clCreateBuffer(context, CL_MEM_READ_ONLY | CL_MEM_COPY_HOST_PTR, (strlength + 1) * sizeof(char), (void *)input, NULL);
			//cl_mem outputBuffer = clCreateBuffer(context, CL_MEM_WRITE_ONLY, (strlength + 1) * sizeof(char), NULL, NULL);

			//Running the kernel.
			size_t global_work_size[1] = { n };
			state = clEnqueueNDRangeKernel(commandQueue, kernel, 1, NULL, global_work_size, NULL, 0, NULL, NULL);

			/*Step 11: Read the cout put back to host memory.*/
			clEnqueueReadBuffer(commandQueue, d_c, CL_TRUE, 0, n * sizeof(double), h_c, 0, NULL, NULL);

			/*Step 12: Clean the resources.*/
			clReleaseKernel(kernel);				//Release kernel.
			clReleaseProgram(program);				//Release the program object.
			clReleaseMemObject(d_a);		//Release mem object.
			clReleaseMemObject(d_c);
			clReleaseCommandQueue(commandQueue);	//Release  Command queue.
			clReleaseContext(context);				//Release context.


			cli::array<System::Double^>^ local = gcnew cli::array<System::Double^>(n);

			for (i = 0; i < n; i++)
			{
				local[i] = h_c[i];
			}

			return local;
		}
	};


}
