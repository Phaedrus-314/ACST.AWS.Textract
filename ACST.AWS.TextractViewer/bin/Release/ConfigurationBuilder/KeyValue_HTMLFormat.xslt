<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <html>
      <body>
        <h1 align="center">yyField Value Data</h1>
        <table border="3" align="center" >
          <tr>
            <th>Key-Y</th>
            <th>Key-X</th>
            <th>Value-Y</th>
            <th>Value-X</th>
            <th>Key</th>
            <th>Value</th>
            <th>Confidence</th>
            <th>Key-BoundingBox</th>
            <th>Value-BoundingBox</th>
            <!--<th>Id</th>-->
          </tr>
          <xsl:for-each select="ArrayOfPage/Page/Form/Fields/Field">
            <xsl:sort select="Key/Geometry/Center/Y"/>
            <xsl:sort select="Key/Geometry/Center/X"/>
            <tr>
              <td>
                <xsl:value-of select="Key/Geometry/Center/Y"/>
              </td>
              <td>
                <xsl:value-of select="Key/Geometry/Center/X"/>
              </td>

              <td>
                <xsl:value-of select="Value/Geometry/Center/Y"/>
              </td>
              <td>
                <xsl:value-of select="Value/Geometry/Center/X"/>
              </td>
              <td>
                <xsl:value-of select="Key/Text"/>
              </td>
              <td>
                <xsl:value-of select="Value/Text"/>
              </td>
              
              <td>
                <xsl:value-of select="Value/Confidence"/>
              </td>

              <td>
                ([<xsl:value-of select="Key/Geometry/BoundingBox/Top"/>,<xsl:value-of select="Key/Geometry/BoundingBox/Left"/>]
                -[<xsl:value-of select="Key/Geometry/BoundingBox/Height"/>,<xsl:value-of select="Key/Geometry/BoundingBox/Width"/>])
              </td>
              <td>
                ([<xsl:value-of select="Value/Geometry/BoundingBox/Top"/>,<xsl:value-of select="Value/Geometry/BoundingBox/Left"/>]
                -[<xsl:value-of select="Value/Geometry/BoundingBox/Height"/>,<xsl:value-of select="Value/Geometry/BoundingBox/Width"/>])
              </td>

              <!--<td>
                <xsl:value-of select="Value/Id"/>
              </td>-->
            </tr>
          </xsl:for-each>
        </table>

        <h1 align="center">Table Data</h1>
        <table border="3" align="center" >
          <xsl:for-each select="ArrayOfPage/Page/Tables/Table/Rows/Row[position() = 1]">
            <tr>
                <xsl:for-each select="Cells/Cell">
                    <th>
                      <xsl:value-of select="Text"/>
                    </th>
                </xsl:for-each>
            </tr>
          </xsl:for-each>
          <xsl:for-each select="ArrayOfPage/Page/Tables/Table/Rows/Row[position() &gt; 1]">
            <tr>
              <xsl:for-each select="Cells/Cell">
                <td>
                  <xsl:value-of select="Text"/>
                </td>
              </xsl:for-each>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
