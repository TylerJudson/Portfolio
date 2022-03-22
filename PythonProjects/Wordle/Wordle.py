from turtle import fillcolor
import pygame
from pygame.locals import *
from Text import Text
from TextBox import TextBox
from colors import *
from Window import Window
from Surface import Surface
from Button import Button
from HoverStyle import HoverStyle


class Wordle:
	"""A class to play the game Wordle
	"""

	size = (400, 550)
	"""The size of the game window"""
	width = size[0]
	"""The width of the game window"""
	height = size[1]
	"""The height of the game window"""


	caption = "WORDLE"
	"""The caption for the game window"""
	backgroundColor = (23, 23, 23)
	"""The background color for the game window"""
	clock = pygame.time.Clock()
	"""The clock for the game to keep track of the time between each frame"""
	font = "HelveticaNeueBold.ttf"
	"""The font for the game"""

	def __init__(self):
		"""Initializes the Game
		"""
		# Initializes pygame and creates the window
		pygame.init()
		self.window = Window(self.size, self.caption, self.backgroundColor)

	def Start(self):
		"""Shows the start screen for the game
		"""
		# create the startScreen		
		startScreen = Surface((0, 0), self.size, self.backgroundColor)

		# Create the title for the startScreen
		title = Text((self.width / 2, 100), self.font, 75, "WORDLE", WHITE)

		# Create the Log in button for the startScreen
		logInButton = Button((self.width / 4 - 75 + 5, 400), Surface((0, 0), (150, 50), self.backgroundColor),
								Text((75, 25), self.font, 25, "LOG IN", DENIM), fillColor=self.backgroundColor,
								border=True, borderColor=DENIM,
								hoverStyle=HoverStyle(Text((75, 25), self.font, 26, "LOG IN", WHITE), fillColor=DENIM, borderColor=DENIM),
								borderRadius=5)

		# Create the sign up button for the startScreen
		signUpButton = Button((self.width * 3 / 4 - 75 - 5, 400), Surface((0, 0), (150, 50), self.backgroundColor),
								Text((75, 25), self.font, 24, "SIGN UP", ORCHID), fillColor=self.backgroundColor,
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
			startScreen.display.blit(title.display, title.rect)

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
		""" Shows the log in screen for the game
		"""
		# create the log in screen
		logInScreen = Surface((0, 0), self.size, self.backgroundColor)
		


		# Create the header for the screen

		# create the header
		header = Surface((0, 0), (self.width, 35), CYBERGRAPE)

		# create the button to go back to the home page that doubles as the title
		titleButton = Button((header.width / 2 - 75, 2), Surface((0, 0), (150, 35), CYBERGRAPE),
								Text((150 / 2, 35 / 2), self.font, 25, "WORDLE", WHITE), fillColor=CYBERGRAPE)



		# Create the title for the screen
		title = Text((self.width / 2, 125), self.font, 75, "LOG IN", WHITE)

		# Create the Username label for the screen
		usernameLbl = Text((self.width / 4, 200), self.font, 20, "USERNAME", WHITE)

		# Create the username text box for the screen
		usernameTxt = TextBox((self.width / 4 - 50, 225), Surface((0, 0), (300, 25), self.backgroundColor),
								Text((75, 25 / 2), self.font, 25, "username", BLACK),
								borderColor=DENIM, borderRadius=5)

		# Create the Password label for the screen
		passwordLbl = Text((self.width / 4, 300), self.font, 20, "PASSWORD", WHITE)


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
			logInScreen.display.blit(title.display, title.rect)

			# render the username label
			logInScreen.display.blit(usernameLbl.display, usernameLbl.rect)

			# render the username Text box
			usernameTxt.render()
			logInScreen.display.blit(usernameTxt.surface.display, usernameTxt.pos)

			# render the password label
			logInScreen.display.blit(passwordLbl.display, passwordLbl.rect)
			
			# render the screen
			self.window.display.blit(logInScreen.display, logInScreen.pos)

			# update
			pygame.display.update()
		

	def SignUp(self):
		"""Shows the sign up screen for the game
		"""
		print("SIGNING UP")



wordle = Wordle()
wordle.Start()