import pygame
from pygame.locals import *
from Surface import Surface
		
class Button:
	def __init__(self, pos, surface, text, font, fontSize, color, fill=True, fillColor=(0, 0, 0), border=False, borderColor=(0, 0, 0), borderRadius=0, hoverStyle=None):
		self.pos = pos
		self.surface = surface
		
		self.text = text
		self.font = font
		self.fontSize = fontSize
		self.color = color
		
		self.fill = fill
		self.fillColor = fillColor
		
		self.border = border
		self.borderColor = borderColor

		self.borderRadius = borderRadius

		self.hoverStyle = hoverStyle
		
		self.rect = Rect(0, 0, self.surface.size[0], self.surface.size[1])

	def render(self, mousePos):
		# clear the surface
		self.surface.clear()
		
		
		# If we want to hover and the mouse is hovering over the button
		# Changes the style of the button
		if (self.hoverStyle != None and self.mouseIsHovering(mousePos)):

			# fills the button
			if (self.hoverStyle.fillColor != None):
				pygame.draw.rect(self.surface.display, self.hoverStyle.fillColor, self.rect, 0, self.borderRadius)

			# makes the border
			if (self.hoverStyle.borderColor != None):
				pygame.draw.rect(self.surface.display, self.hoverStyle.borderColor, self.rect, 2, self.borderRadius)


			# place the text
				
			# Create the font
			font = pygame.font.Font(self.font, self.fontSize)
	
			# Set the text
			text = font.render(self.text, True, self.hoverStyle.color)
	
			# Get the rectangle of the text for centering
			textRect = text.get_rect(center=(self.surface.size[0]/2, self.surface.size[1]/2))
	
			# render the text on the screen
			self.surface.display.blit(text, textRect)

			
		# Else draw the button normally
		else:
			# fills the button
			if (self.fill):
				pygame.draw.rect(self.surface.display, self.fillColor, self.rect, 0, self.borderRadius)

			# makes the border
			if (self.border):
				pygame.draw.rect(self.surface.display, self.borderColor, self.rect, 2, self.borderRadius)


			# place the text
				
			# Create the font
			font = pygame.font.Font(self.font, self.fontSize)
	
			# Set the text
			text = font.render(self.text, True, self.color)
	
			# Get the rectangle of the text for centering
			textRect = text.get_rect(center=(self.surface.size[0]/2, self.surface.size[1]/2))
	
			# render the text on the screen
			self.surface.display.blit(text, textRect)

	def mouseIsHovering(self, mousePos):
		# If the mouse pos is inside of the 
		if (self.pos[0] <= mousePos[0] <= self.pos[0] + self.surface.size[0] and self.pos[1] <= mousePos[1] <= self.pos[1] + self.surface.size[1]):
			return True

		return False