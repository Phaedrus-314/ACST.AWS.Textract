<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <!--<xsl:output method="text" omit-xml-declaration="yes" indent="no"/>-->

  <!--'new NamedCoordinate(&quot;HeaderInfo&quot;, &quot;Header_ActualServices&quot;' +
  ', &quot;' + ExactTextMatch + '&quot;' +
  ', new PointF { X = ' + IdealCenterKey.X +
  ', Y = ' + IdealCenterKey.Y + '}' +
  ', new PointF { X = ' + IdealCenterValue.X +
  ', Y = ' + IdealCenterValue.Y + '}' +
  '))'-->

  <xsl:template match="/">
    <Method xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
      <xsl:for-each select="ArrayOfPage/Page/Form/Fields/Field">
        <xsl:sort select="Key/Geometry/Center/Y"/>
        <xsl:sort select="Key/Geometry/Center/X"/>
        <call>
          <xsl:value-of select="concat(
                    'namedCoordinates.Add(new NamedCoordinate(&quot;gn&quot;, &quot;n&quot;'
                    ,', &quot;'
                    ,Key/Text
                    ,'&quot;'
                    ,', &quot;'
                    ,Value/Text
                    ,'&quot;'
                    ,', new PointF { X = '
                    ,Key/Geometry/Center/X
                    ,'0F, Y = '
                    ,Key/Geometry/Center/Y
                    ,'0F}'
                    ,', new PointF { X = '
                    ,Value/Geometry/Center/X
                    ,'0F, Y = '
                    ,Value/Geometry/Center/Y
                    ,'0F}'
			              ,', new BoundingBox { Height = '
			              ,Key/Geometry/BoundingBox/Height
			              ,'0F, Left = '
			              ,Key/Geometry/BoundingBox/Left
			              ,'0F, Top = '
			              ,Key/Geometry/BoundingBox/Top
			              ,'0F, Width = '
			              ,Key/Geometry/BoundingBox/Width
			              ,', new BoundingBox { Height = '
			              ,Value/Geometry/BoundingBox/Height
			              ,'0F, Left = '
			              ,Value/Geometry/BoundingBox/Left
			              ,'0F, Top = '
			              ,Value/Geometry/BoundingBox/Top
			              ,'0F, Width = '
			              ,Value/Geometry/BoundingBox/Width
                    ,'0F}'
                    ,'));'
                    ,'&#xA;')"/>
        </call>
      </xsl:for-each>
    </Method>
  </xsl:template>
  <!--
    <xsl:template match="ArrayOfPage/Page/Form/Fields/Field">
    <dd xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
    <xsl:value-of select="concat(
                    'new NamedCoordinate(&quot;HeaderInfo&quot;, &quot;Header_ActualServices&quot;'
                    ,', &quot;'
                    ,ExactTextMatch
                    ,'&quot;'
                    ,Key/Geometry/Center/X
                    ,':'
                    ,Key/Geometry/Center/Y
                    ,' '
                    ,Value/Geometry/Center/X
                    ,'&#xA;')"/>
    </dd>
  </xsl:template>
  -->
  
</xsl:stylesheet>
