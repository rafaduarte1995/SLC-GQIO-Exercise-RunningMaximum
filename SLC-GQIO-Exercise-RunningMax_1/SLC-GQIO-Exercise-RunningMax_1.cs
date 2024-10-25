/*
****************************************************************************
*  Copyright (c) 2024,  Skyline Communications NV  All Rights Reserved.    *
****************************************************************************

By using this script, you expressly agree with the usage terms and
conditions set out below.
This script and all related materials are protected by copyrights and
other intellectual property rights that exclusively belong
to Skyline Communications.

A user license granted for this script is strictly for personal use only.
This script may not be used in any way by anyone without the prior
written consent of Skyline Communications. Any sublicensing of this
script is forbidden.

Any modifications to this script by the user are only allowed for
personal use and within the intended purpose of the script,
and will remain the sole responsibility of the user.
Skyline Communications will not be responsible for any damages or
malfunctions whatsoever of the script resulting from a modification
or adaptation by the user.

The content of this script is confidential information.
The user hereby agrees to keep this confidential information strictly
secret and confidential and not to disclose or reveal it, in whole
or in part, directly or indirectly to any person, entity, organization
or administration without the prior written consent of
Skyline Communications.

Any inquiries can be addressed to:

	Skyline Communications NV
	Ambachtenstraat 33
	B-8870 Izegem
	Belgium
	Tel.	: +32 51 31 35 69
	Fax.	: +32 51 31 01 29
	E-mail	: info@skyline.be
	Web		: www.skyline.be
	Contact	: Ben Vandenberghe

****************************************************************************
Revision History:

DATE		VERSION		AUTHOR			COMMENTS

19/04/2024	1.0.0.1		JarnoLE, Skyline	Initial version
****************************************************************************
*/

namespace SLC_GQIO_Exercise_RunningTotal_1
{
	using System;

	using Skyline.DataMiner.Analytics.GenericInterface;

	[GQIMetaData(Name = "Running Total")]
	public class MyCustomOperator : IGQIColumnOperator, IGQIRowOperator, IGQIInputArguments
	{
		private readonly GQIColumnDropdownArgument _firstColumnArg = new GQIColumnDropdownArgument("First Column") { IsRequired = true, Types = new GQIColumnType[] { GQIColumnType.Double } };
		private readonly GQIStringArgument _columnNameArg = new GQIStringArgument("RunningMax Column Name") { IsRequired = true };

		private GQIColumn _firstColumn;
		private string _columnName;
		private GQIDoubleColumn _newColumnTotalCount;
		private GQIDoubleColumn _newColumnRunningMax;

		private double totalCount = 0;

		private double runningMax = 0;

		public GQIArgument[] GetInputArguments()
		{
			return new GQIArgument[] { _firstColumnArg, _columnNameArg };
		}

		public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
		{
			_firstColumn = args.GetArgumentValue(_firstColumnArg);
			_columnName = args.GetArgumentValue(_columnNameArg);
			_newColumnTotalCount = new GQIDoubleColumn("Running Total");
			_newColumnRunningMax = new GQIDoubleColumn(_columnName);

			return new OnArgumentsProcessedOutputArgs();
		}

		public void HandleColumns(GQIEditableHeader header)
		{
			header.AddColumns(_newColumnTotalCount, _newColumnRunningMax);
		}

		public void HandleRow(GQIEditableRow row)
		{
			var firstValueDouble = row.GetValue<double>(_firstColumn);
			totalCount += firstValueDouble;
			
			// Running Max
			if(firstValueDouble > runningMax)
			{
				runningMax = firstValueDouble;
			}

			var totalCountResult = totalCount;
			var runningMaxResult = runningMax;
			var totalCountRounded = Math.Round(totalCountResult, 2);
			var runningMaxRonded	= Math.Round(runningMaxResult, 2);

			row.SetValue(_newColumnTotalCount, totalCountResult, $"{totalCountRounded}");
			row.SetValue(_newColumnRunningMax, runningMaxResult, $"{runningMaxRonded}");
		}
	}
}
