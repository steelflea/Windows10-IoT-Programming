#include "pch.h"
#include "ObjectDescriptor.h"

using namespace ImageProcessingComponent;
using namespace Windows::Foundation;

ObjectDescriptor::ObjectDescriptor()
{
	points = nullptr;
	area = 0.0;
}

ObjectDescriptor::ObjectDescriptor(vector<cv::Point> contour, double area)
{
	auto contourSize = contour.size();
	if (contourSize > 0)
	{
		points = ref new Vector<Point>();

		for (int i = 0; i < contourSize; i++)
		{
			auto cvPoint = contour.at(i);

			points->Append(Point(cvPoint.x, cvPoint.y));
		}
	}

	this->area = area;
}
