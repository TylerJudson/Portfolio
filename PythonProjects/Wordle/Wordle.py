from turtle import fillcolor
import pygame
from pygame.locals import *
from Text import Text
from colors import *
from Window import Window
from Surface import Surface
from Button import Button
from HoverStyle import HoverStyle


class Wordle:

	size = (400, 550)
	width = size[0]
	height = size[1]


	caption = "WORDLE"
	backgroundColor = (23, 23, 23)
	clock = pygame.time.Clock()
	font = "HelveticaNeueBold.ttf"

	def __init__(self):
		pygame.init()
		self.window = Window(self.size, self.caption, self.backgroundColor)


	def Start(self):
		# create the startScreen		
		startScreen = Surface((0, 0), self.size, self.backgroundColor)

		# Create the title for the startScreen
		title = Text((self.width / 2, 100), self.font, 75, "WORDLE", WHITE)

		# Create the Log in button for the startScreen
		logInButton = Button((self.width / 4 - 75 + 5, 400), Surface((0, 0), (150, 50), self.backgroundColor),
								Text((75, 25), self.font, 25, "LOG IN", DENIM),
								border=True, borderColor=DENIM,
								hoverStyle=HoverStyle(Text((75, 25), self.font, 25, "LOG IN", WHITE), fillColor=DENIM, borderColor=DENIM),
								borderRadius=5)

		# Create the sign up button for the startScreen
		signUpButton = Button((self.width * 3 / 4 - 75 - 5, 400), Surface((0, 0), (150, 50), self.backgroundColor),
								Text((75, 25), self.font, 25, "SIGN UP", ORCHID),
								border=True, borderColor=ORCHID,
								hoverStyle=HoverStyle(Text((75, 25), self.font, 25, "SIGN UP", WHITE), fillColor=ORCHID, borderColor=ORCHID),
								borderRadius=5)

		while True:
			# run at 60 fps
			self.clock.tick(60)

			# get the position of the mouse for later use
			mousePos = pygame.mouse.get_pos()

			# loop through the events
			for event in pygame.event.get():

				# Check for QUIT event
				if event.type == pygame.QUIT:
					return

				# Checks for the MOUSEDOWN event
				if event.type == pygame.MOUSEBUTTONDOWN:
					# if the mouse clicked the log in button
					if (logInButton.mouseIsHovering(mousePos)):
						self.LogIn()
						return

					# if the mouse clicked the sign up button
					elif (signUpButton.mouseIsHovering(mousePos)):
						self.SignUp()
						return

			# render the title
			startScreen.display.blit(title.surface, title.rect)

			# render the buttons

			# render the log in button
			logInButton.render(mousePos)
			startScreen.display.blit(logInButton.surface.display, logInButton.pos)

			# render the sign up button
			signUpButton.render(mousePos)
			startScreen.display.blit(signUpButton.surface.display, signUpButton.pos)

			# render the screen
			self.window.display.blit(startScreen.display, startScreen.pos)

			# update
			pygame.display.update()


	
	def LogIn(self):
		# create the log in screen
		logInScreen = Surface((0, 0), self.size, self.backgroundColor)


		
		# Create the header for the screen

		# create the header
		header = Surface((0, 0), (self.width, 35), CYBERGRAPE)

		# create the button to go back to the home page that doubles as the title
		titleButton = Button((header.width / 2 - 75, 2), Surface((0, 0), (150, 35), CYBERGRAPE),
								Text((150 / 2, 35 / 2), self.font, 25, "WORDLE", WHITE))

		# Create the title for the screen
		title = Text((self.width / 2, 125), self.font, 75, "LOG IN", WHITE)

		
		while True:
			# run at 60 fps
			self.clock.tick(60)

			# get the position of the mouse for later use
			mousePos = pygame.mouse.get_pos()

			# loop through the events
			for event in pygame.event.get():

				# Check for QUIT event
				if event.type == pygame.QUIT:
					return

				# Checks for the MOUSEDOWN event
				if event.type == pygame.MOUSEBUTTONDOWN:
					# if the mouse clicked the log in button
					if (titleButton.mouseIsHovering(mousePos)):
						self.Start()
						return

				
			# render the header
			titleButton.render()
			header.display.blit(titleButton.surface.display, titleButton.pos)
			logInScreen.display.blit(header.display, header.pos)

			# render the title
			logInScreen.display.blit(title.surface, title.rect)
			
			# render the screen
			self.window.display.blit(logInScreen.display, logInScreen.pos)

			# update
			pygame.display.update()
		

	def SignUp(self):
		print("SIGNING UP")



wordle = Wordle()
wordle.Start()