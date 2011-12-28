<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="Kinectonitor.Azure" generation="1" functional="0" release="0" Id="e0c453ff-abbb-4f36-9c1e-e3d475fa9a34" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="Kinectonitor.AzureGroup" generation="1" functional="0" release="0">
      <settings>
        <aCS name="Kinectonitor.Azure.Worker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Kinectonitor.Azure/Kinectonitor.AzureGroup/MapKinectonitor.Azure.Worker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="Kinectonitor.Azure.Worker:StorageConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Kinectonitor.Azure/Kinectonitor.AzureGroup/MapKinectonitor.Azure.Worker:StorageConnectionString" />
          </maps>
        </aCS>
        <aCS name="Kinectonitor.Azure.WorkerInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/Kinectonitor.Azure/Kinectonitor.AzureGroup/MapKinectonitor.Azure.WorkerInstances" />
          </maps>
        </aCS>
      </settings>
      <maps>
        <map name="MapKinectonitor.Azure.Worker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Kinectonitor.Azure/Kinectonitor.AzureGroup/Kinectonitor.Azure.Worker/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapKinectonitor.Azure.Worker:StorageConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Kinectonitor.Azure/Kinectonitor.AzureGroup/Kinectonitor.Azure.Worker/StorageConnectionString" />
          </setting>
        </map>
        <map name="MapKinectonitor.Azure.WorkerInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/Kinectonitor.Azure/Kinectonitor.AzureGroup/Kinectonitor.Azure.WorkerInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="Kinectonitor.Azure.Worker" generation="1" functional="0" release="0" software="C:\Users\bradyg\Dropbox\Kinectonitor\Kinectonitor.Azure\csx\Debug\roles\Kinectonitor.Azure.Worker" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="1792" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="StorageConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;Kinectonitor.Azure.Worker&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;Kinectonitor.Azure.Worker&quot; /&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/Kinectonitor.Azure/Kinectonitor.AzureGroup/Kinectonitor.Azure.WorkerInstances" />
            <sCSPolicyFaultDomainMoniker name="/Kinectonitor.Azure/Kinectonitor.AzureGroup/Kinectonitor.Azure.WorkerFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyFaultDomain name="Kinectonitor.Azure.WorkerFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="Kinectonitor.Azure.WorkerInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
</serviceModel>